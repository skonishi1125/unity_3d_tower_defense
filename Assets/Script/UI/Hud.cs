using TMPro;
using UnityEngine;

public class Hud : MonoBehaviour
{
    // EconomyManagerなどをSerializeFieldで割り当てて、
    // そこからIEconomyを抜き取り、コードで使う
    // (EconomyManager em としないのは、SerializeFieldで割り当てられないから）
    [SerializeField] private MonoBehaviour economyProvider;
    [SerializeField] private MonoBehaviour lifeProvider;

    [SerializeField] private TextMeshProUGUI moneyAmount;
    [SerializeField] private TextMeshProUGUI lifeAmount;

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

    }

    private void UpdateMoneyAmount(float currentMoney)
    {
        moneyAmount.text = $"{currentMoney} 円";
    }

    private void UpdateLifeAmount(int currentLife)
    {
        lifeAmount.text = $"{currentLife}";
    }

    private void OnEnable()
    {
        if (economy != null)
            economy.MoneyChanged += UpdateMoneyAmount;

        if (life != null)
            life.LifeChanged += UpdateLifeAmount;

    }
    private void OnDisable()
    {
        if (economy != null)
            economy.MoneyChanged -= UpdateMoneyAmount;

        if (life != null)
            life.LifeChanged -= UpdateLifeAmount;

    }


}
