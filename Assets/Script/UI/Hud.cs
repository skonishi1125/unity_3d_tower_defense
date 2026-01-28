using TMPro;
using UnityEngine;

public class Hud : MonoBehaviour
{
    // EconomyManagerなどをSerializeFieldで割り当てて、
    // そこからIEconomyを抜き取り、コードで使う
    // (EconomyManager em としないのは、SerializeFieldで割り当てられないから）
    [SerializeField] private MonoBehaviour economyProvider;
    [SerializeField] private TextMeshProUGUI moneyAmount;

    private IEconomy economy;

    private void Awake()
    {
        // MonoBehaviourをキャスト
        economy = economyProvider as IEconomy;
        if (economy == null)
        {
            Debug.LogError("economyProvider must implement IEconomy.");
            enabled = false;
            return;
        }

        // 初期表示（購読前にAwake通知が終わっている可能性があるため）
        UpdateMoneyAmount(economy.CurrentMoney);
    }

    private void UpdateMoneyAmount(float currentMoney)
    {
        moneyAmount.text = $"{currentMoney} 円";
    }

    private void OnEnable()
    {
        if (economy == null) return;
        economy.MoneyChanged += UpdateMoneyAmount;
        UpdateMoneyAmount(economy.CurrentMoney);
    }
    private void OnDisable()
    {
        if (economy == null) return;
        economy.MoneyChanged -= UpdateMoneyAmount;
    }


}
