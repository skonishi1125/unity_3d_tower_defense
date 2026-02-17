using TMPro;
using UnityEngine;
using DG.Tweening;

public class Hud : MonoBehaviour
{
    // ManagerやControllerなど、ゲームシステム系
    // EconomyManagerなどをSerializeFieldで割り当てて、
    // そこからIEconomyを抜き取り、コードで使う
    // ※EconomyManager economyManager などクラス名で定義しないのはSerializeFieldで割り当てられないから
    [Header("Game Systems")]
    [SerializeField] private MonoBehaviour economyProvider;
    [SerializeField] private MonoBehaviour lifeProvider;
    [SerializeField] private StageManager stageManager;
    [SerializeField] private StateManager stateManager;
    [SerializeField] private BuildController buildController;

    [Header("Hud Texts")]
    [SerializeField] private TextMeshProUGUI moneyAmount;
    [SerializeField] private TextMeshProUGUI lifeAmount;
    [SerializeField] private TextMeshProUGUI waveNumber;
    [SerializeField] private TextMeshProUGUI gameStateText;
    [SerializeField] private TextMeshProUGUI buildModeText;
    [SerializeField] private TextMeshProUGUI errorMessage;

    [Header("Number Of Unit Setting")]
    [SerializeField] private TextMeshProUGUI numberOfUnitText;
    [SerializeField] private const float CautionRatio = .6f;

    [Header("UIs")]
    [SerializeField] private GameObject EditStateUI;
    [SerializeField] private GameObject PlayingStateUI;

    private Tween moneyTextTween;
    private Color originalMoneyColor;
    private Vector3 originalMoneyLocalPos;

    private Tween numberOfUnitTextTween;
    private Color originalNumberOfUnitColor;
    private Vector3 originalNumberOfUnitLocalPos;

    private IEconomy economy;
    private ILife life;

    private void Awake()
    {
        // MonoBehaviourをキャスト
        economy = economyProvider as IEconomy;
        if (economy == null)
        {
            Debug.LogError("economyProvider には IEconomyが必須です。");
            enabled = false;
            return;
        }

        life = lifeProvider as ILife;
        if (life == null)
        {
            Debug.LogError("lifeProvider には ILifeが必須です。");
            enabled = false;
            return;
        }

        if (stageManager == null)
        {
            Debug.Log("Hud: stageManager自動割当");
            stageManager = FindFirstObjectByType<StageManager>();
        }

        if (stateManager == null)
        {
            Debug.Log("Hud: ステートManager自動割当");
            stateManager = FindFirstObjectByType<StateManager>();
        }

        if (buildController == null)
        {
            Debug.Log("Hud: BuildController自動割当");
            buildController = FindFirstObjectByType<BuildController>();
        }

        // テキスト初期設定
        SetUpText();

    }


    private void OnEnable()
    {
        if (economy != null)
        {
            economy.MoneyChanged += UpdateMoneyAmount;
            economy.OnInsufficientFunds += HandleDisplayErrorFund;
            economy.OnInsufficientFunds += HandleAnimateInsufficientFunds;
            // ラムダで書くと、こんな感じにはなるがアンチパターンである
            // economy.OnInsufficientFunds += _ => AnimateInsufficientFunds();
        }

        if (life != null)
            life.LifeChanged += UpdateLifeAmount;

        if (stageManager != null)
            stageManager.WaveChanged += UpdateWaveText;

        if (stateManager != null)
        {
            stateManager.StateChanged += UpdateGameStateText;
            stateManager.StateChanged += UpdateGameStateUI;
        }

        if (buildController != null)
        {
            buildController.BuildModeChanged += UpdateBuildModeText;
            buildController.BulidUnitChanged += UpdateNumberOfUnitText;
            buildController.BuildErrorOccured += HandleDisplayErrorBuild;
            buildController.BuildErrorOccured += HandleAnimateMaxUnitNumber;
        }
    }

    private void OnDisable()
    {
        if (economy != null)
        {
            economy.MoneyChanged -= UpdateMoneyAmount;
            economy.OnInsufficientFunds -= HandleDisplayErrorFund;
            economy.OnInsufficientFunds -= HandleAnimateInsufficientFunds;
        }

        if (life != null)
            life.LifeChanged -= UpdateLifeAmount;

        if (stageManager != null)
            stageManager.WaveChanged -= UpdateWaveText;

        if (stateManager != null)
        {
            stateManager.StateChanged -= UpdateGameStateText;
            stateManager.StateChanged -= UpdateGameStateUI;
        }

        if (buildController != null)
        {
            buildController.BuildModeChanged -= UpdateBuildModeText;
            buildController.BulidUnitChanged -= UpdateNumberOfUnitText;
            buildController.BuildErrorOccured -= HandleDisplayErrorBuild;
            buildController.BuildErrorOccured -= HandleAnimateMaxUnitNumber;
        }
    }

    private void SetUpText()
    {
        if (lifeAmount != null)
            UpdateLifeAmount(life.CurrentLife);

        if (waveNumber != null)
            UpdateWaveText();

        if (gameStateText != null)
            UpdateGameStateText();

        if (buildModeText != null)
            UpdateBuildModeText();

        if (moneyAmount != null)
        {
            UpdateMoneyAmount(economy.CurrentMoney);
            originalMoneyColor = moneyAmount.color;
            originalMoneyLocalPos = moneyAmount.transform.localPosition;
        }

        if (errorMessage != null)
            errorMessage.text = ""; // Object自体を消すのではなく、テキストだけ空にする

        if (numberOfUnitText != null)
        {
            UpdateNumberOfUnitText();
            originalNumberOfUnitColor = numberOfUnitText.color;
            originalNumberOfUnitLocalPos = numberOfUnitText.transform.localPosition;
        }

    }

    // Stateに応じたUIの切り替え
    private void UpdateGameStateUI()
    {
        GameState state = stateManager.State;

        // 全てのルートパネルを初期化し、stateに応じて呼び分ける
        EditStateUI.SetActive(state == GameState.Edit);
        PlayingStateUI.SetActive(state == GameState.Playing);
    }

    private void UpdateBuildModeText()
    {
        if (buildController.CurrentBuildMode == BuildMode.Build)
        {
            buildModeText.color = Color.white;
            buildModeText.text = "建築";
        }
        else if (buildController.CurrentBuildMode == BuildMode.Demolish)
        {
            buildModeText.color = Color.red;
            buildModeText.text = "壊す";
        }
        else
        {
            buildModeText.text = " - ";
        }
    }

    private void UpdateGameStateText()
    {
        if (stateManager.State == GameState.Edit)
        {
            gameStateText.color = Color.white;
            gameStateText.text = "【EDIT】";
        }
        else if (stateManager.State == GameState.Playing)
        {
            gameStateText.color = Color.red;
            gameStateText.text = "【PLAY】";
        }
    }

    private void UpdateMoneyAmount(float currentMoney)
    {
        float number = currentMoney;
        string formattedNumber = number.ToString("N0");
        moneyAmount.text = $"¥ {formattedNumber}";
    }

    private void UpdateLifeAmount(int currentLife)
    {
        string lifeSquare = "■";
        string currentLifeSquare = "";
        for (int i = 0; i < currentLife; i++)
        {
            currentLifeSquare = currentLifeSquare + lifeSquare;
        }
        lifeAmount.text = currentLifeSquare;
    }

    private void UpdateWaveText()
    {
        waveNumber.text = $"Wave: {stageManager.CurrentWaveIndex + 1} / {stageManager.MaxWave}";
    }

    private void HandleAnimateInsufficientFunds(string _)
    {
        AnimateInsufficientFunds();
    }

    // お金が足りないとき、赤くして、揺らす
    private void AnimateInsufficientFunds()
    {
        if (moneyAmount == null) return;

        // 連打対策
        // 前のアニメーションがあればKillして、改めて走らせる
        // 色と位置を元の場所にリセット
        moneyTextTween?.Kill();
        moneyAmount.transform.DOKill();
        moneyAmount.color = originalMoneyColor;
        moneyAmount.transform.localPosition = originalMoneyLocalPos;

        moneyTextTween = moneyAmount.DOColor(Color.red, 0.1f)
            .SetUpdate(true) // TimeScale = 0 でも動かす
            .SetLoops(4, LoopType.Yoyo)
            .OnComplete(() => moneyAmount.color = originalMoneyColor)
            .SetLink(gameObject);

        // テキスト自体の揺れモーション
        // .3秒で、y軸に揺らす
        moneyAmount.transform.DOShakePosition(0.3f, new Vector3(0f, 5f, 0f))
            .SetUpdate(true)
            .SetLink(gameObject);
    }

    private void HandleAnimateMaxUnitNumber(string _, BuildErrorType type)
    {
        AnimateMaxUnitNumber(type);
    }

    // 最大建設上限に達したとき、ユニットの表記を赤くして、揺らす
    private void AnimateMaxUnitNumber(BuildErrorType type)
    {
        if (numberOfUnitText == null) return;
        if (type != BuildErrorType.Maximum) return;

        numberOfUnitTextTween?.Kill();
        numberOfUnitText.transform.DOKill();
        numberOfUnitText.color = originalNumberOfUnitColor;
        numberOfUnitText.transform.localPosition = originalNumberOfUnitLocalPos;

        numberOfUnitTextTween = numberOfUnitText.DOColor(Color.red, 0.1f)
            .SetUpdate(true) // TimeScale = 0 でも動かす
            .SetLoops(4, LoopType.Yoyo)
            .OnComplete(() => numberOfUnitText.color = originalNumberOfUnitColor)
            .SetLink(gameObject);

        numberOfUnitText.transform.DOShakePosition(0.3f, new Vector3(0f, 5f, 0f))
            .SetUpdate(true)
            .SetLink(gameObject);
    }

    private void HandleDisplayErrorFund(string message)
    {
        DisplayErrorMessage(message);
    }

    private void HandleDisplayErrorBuild(string message, BuildErrorType _) {
        DisplayErrorMessage(message);
    }

    private void DisplayErrorMessage(string message)
    {
        errorMessage.DOKill();
        errorMessage.alpha = 1f;
        errorMessage.text = message;

        errorMessage.DOFade(0f, 1f)
            .SetDelay(3f)
            .OnComplete(() =>
            {
                // アニメーション完了後
                errorMessage.text = "";
                errorMessage.alpha = 1f;
            })
            .SetUpdate(true)
            .SetLink(gameObject); // このUIが消えたらTweenも消す
    }

    private void UpdateNumberOfUnitText()
    {
        int currentNum = buildController.CurrentUnitNumber;
        int MaxNum = buildController.MaxBuildableUnitNumber;

        numberOfUnitText.text = $"{currentNum}/{MaxNum}";

        // (float) (currentNum / MaxNum)とはしない。
        // 整数同士の割り算は小数点以下が切り捨てられるので、今回 0 or 1しか返ってこない
        // currentNumにのみfloatでキャストしてやれば、
        // c#側がfloat / intの計算として、floatを返してくれるようになる
        float ratio = (float)currentNum / MaxNum;

        numberOfUnitText.color = ratio switch
        {
            >= 1.0f => Color.red, // 上限(100%)赤
            >= CautionRatio => Color.yellow, // 黄色
            _ => Color.white // デフォルトで白
        };

    }

}
