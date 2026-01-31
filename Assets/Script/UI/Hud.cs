using TMPro;
using UnityEngine;

public class Hud : MonoBehaviour
{
    // インターフェース系
    // EconomyManagerなどをSerializeFieldで割り当てて、
    // そこからIEconomyを抜き取り、コードで使う
    // (EconomyManager em としないのは、SerializeFieldで割り当てられないから）
    [SerializeField] private MonoBehaviour economyProvider;
    [SerializeField] private MonoBehaviour lifeProvider;
    [SerializeField] private StageManager stageManager;

    [SerializeField] private TextMeshProUGUI moneyAmount;
    [SerializeField] private TextMeshProUGUI lifeAmount;
    [SerializeField] private TextMeshProUGUI waveNumber;

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
            stageManager = FindFirstObjectByType<StageManager>();

        if (waveNumber != null)
            UpdateWaveText();

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

    }
    private void OnDisable()
    {
        if (economy != null)
            economy.MoneyChanged -= UpdateMoneyAmount;

        if (life != null)
            life.LifeChanged -= UpdateLifeAmount;

        if (stageManager != null)
            stageManager.WaveChanged -= UpdateWaveText;
    }


}
