using System;

public interface IEconomy
{
    // インターフェースはアクセスレベルは基本的に使わない
    // (ごく例外もあるが、基本publicとして使う）
    // IEconomyを持つクラスは、これらの実装を必ず持つというルール
    int CurrentMoney { get; }

    event Action<int> MoneyChanged;
    event Action<string> OnInsufficientFunds; // 支払い失敗のイベント

    bool TrySpend(int cost);

    void Refund(int cost);

    void AddMoney(int value);
}
