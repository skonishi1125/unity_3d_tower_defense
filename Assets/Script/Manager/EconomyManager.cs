using System;
using UnityEngine;

// sealed: 継承禁止
// class EconomyManagerChild : EcomonyManager とできなくなる
// なお、こうでないと動かないというわけではなく、勉強として一応書いただけ
public sealed class EconomyManager : MonoBehaviour, IEconomy
{
    [Header("Initial Values")]
    [SerializeField] private int initialMoney = 100;

    private float money; // このManager内で使うだけの変数

    [Header("Broadcasting Events")]
    [SerializeField] private IntEventChannelSO onEnemyDiedChannel;

    public float CurrentMoney => money;// 外部から現在金額を参照するため専用の変数
    // moneyとCurrentMoneyの関係は、下記設計と似ている
    // public float CurrentMoney { get; private set; }

    public event Action<float> MoneyChanged; // HUD更新用
    public event Action<string> OnInsufficientFunds;

    private void Awake()
    {
        money = initialMoney;
    }

    private void Start()
    {
        MoneyChanged?.Invoke(money);
    }

    private void OnEnable()
    {
        if (onEnemyDiedChannel != null)
            onEnemyDiedChannel.OnEventRaised += AddMoney;
    }

    private void OnDisable()
    {
        if (onEnemyDiedChannel != null)
            onEnemyDiedChannel.OnEventRaised -= AddMoney;
    }

    public void AddMoney(int amount)
    {
        money += amount;
        MoneyChanged?.Invoke(money);
    }

    public bool TrySpend(int cost)
    {
        if (cost < 0f)
        {
            Debug.LogError("costがマイナスです");
            return false;
        }

        if (money < cost)
        {
            OnInsufficientFunds?.Invoke("所持金が足りません。");
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
