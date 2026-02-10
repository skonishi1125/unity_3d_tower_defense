using System;
using UnityEngine;

// sealed: 継承禁止
// class EconomyManagerChild : EcomonyManager とできなくなる
// なお、こうでないと動かないというわけではなく、勉強として一応書いただけ
public sealed class EconomyManager : MonoBehaviour, IEconomy
{
    [Header("Initial Values")]
    [SerializeField] private float initialMoney = 100f;

    private float money; // このManager内で使うだけの変数
    public float CurrentMoney => money;// 外部から現在金額を参照するため専用の変数
    // moneyとCurrentMoneyの関係は、下記設計と似ている
    // public float CurrentMoney { get; private set; }

    public event Action<float> MoneyChanged;
    public event Action OnInsufficientFunds;

    private void Awake()
    {
        money = initialMoney;
    }

    private void Start()
    {
        MoneyChanged?.Invoke(money); // HUDなど更新
    }

    public bool TrySpend(float cost)
    {
        if (cost < 0f)
        {
            Debug.LogError("costがマイナスです");
            return false;
        }

        if (money < cost)
        {
            //Debug.Log($"所持金が足りません。必要: {cost} 所持金: {money}");
            OnInsufficientFunds?.Invoke();
            return false;
        }

        money -= cost;
        MoneyChanged?.Invoke(money);
        return true;
    }

    public void Refund(float cost)
    {
        if (cost < 0f)
        {
            Debug.LogError("costがマイナスです");
            return;
        }

        money += cost;
        MoneyChanged?.Invoke(money);
    }

    public void AddMoney(float value)
    {
        if (value < 0f)
        {
            Debug.LogError("costがマイナスです");
            return;
        }

        money += value;
        MoneyChanged?.Invoke(money);
    }

}
