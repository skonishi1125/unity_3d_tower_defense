using TMPro;
using UnityEngine;

public class Hud : MonoBehaviour
{
    // インターフェース系
    // EconomyManagerなどをSerializeFieldで割り当てて、
    // そこからIEconomyを抜き取り、コードで使う
    // (EconomyManager economyManager としないのは、SerializeFieldで割り当てられないから）
    [SerializeField] private MonoBehaviour economyProvider;
    [SerializeField] private MonoBehaviour lifeProvider;
    [SerializeField] private StageManager stageManager;
    [SerializeField] private StateManager stateManager;
    [SerializeField] private BuildController buildController;

    [SerializeField] private TextMeshProUGUI moneyAmount;
    [SerializeField] private TextMeshProUGUI lifeAmount;
    [SerializeField] private TextMeshProUGUI waveNumber;
    [SerializeField] private TextMeshProUGUI gameStateText;
    [SerializeField] private TextMeshProUGUI buildModeText;

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
        // 初期表示（購読前にAwake通知が終わっている可能性があるため）
        UpdateMoneyAmount(economy.CurrentMoney);

        life = lifeProvider as ILife;
        if (life == null)
        {
            Debug.LogError("lifeProvider には ILifeが必須です。");
            enabled = false;
            return;
        }
        UpdateLifeAmount(life.CurrentLife);

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

        if (waveNumber != null)
            UpdateWaveText();

        if (gameStateText != null)
            UpdateGameStateText();

        if (buildModeText != null)
            UpdateBuildModeText();

    }

    private void UpdateBuildModeText()
    {
        if (buildController.CurrentBuildMode == BuildMode.Build)
        {
            buildModeText.color = Color.white;
            buildModeText.text = "建設する";
        }
        else if (buildController.CurrentBuildMode == BuildMode.Demolish)
        {
            buildModeText.color = Color.red;
            buildModeText.text = "除去する";
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
            gameStateText.text = "【編集中】";
        }
        else if (stateManager.State == GameState.Playing)
        {
            gameStateText.color = Color.red;
            gameStateText.text = "【戦闘中】";
        }
    }

    private void UpdateMoneyAmount(float currentMoney)
    {
        float number = currentMoney;
        string formattedNumber = number.ToString("N0");
        moneyAmount.text = $"{formattedNumber}";
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
        waveNumber.text = $"Wave: {stageManager.CurrentWave} / {stageManager.MaxWave}";
    }

    private void OnEnable()
    {
        if (economy != null)
            economy.MoneyChanged += UpdateMoneyAmount;

        if (life != null)
            life.LifeChanged += UpdateLifeAmount;

        if (stageManager != null)
            stageManager.WaveChanged += UpdateWaveText;

        if (stateManager != null)
            stateManager.StateChanged += UpdateGameStateText;

        if (buildController != null)
            buildController.BuildModeChanged += UpdateBuildModeText;


    }
    private void OnDisable()
    {
        if (economy != null)
            economy.MoneyChanged -= UpdateMoneyAmount;

        if (life != null)
            life.LifeChanged -= UpdateLifeAmount;

        if (stageManager != null)
            stageManager.WaveChanged -= UpdateWaveText;

        if (stateManager != null)
            stateManager.StateChanged -= UpdateGameStateText;

        if (buildController != null)
            buildController.BuildModeChanged -= UpdateBuildModeText;
    }

}
