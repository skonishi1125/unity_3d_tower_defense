using System;

public interface IEconomy
{
    // インターフェースはアクセスレベルは基本的に使わない
    // (ごく例外もあるが、基本publicとして使う）
    // IEconomyを持つクラスは、これらの実装を必ず持つというルール
    float CurrentMoney { get; }

    event Action<float> MoneyChanged;
    event Action<string> OnInsufficientFunds; // 支払い失敗のイベント

    bool TrySpend(float cost);

    void Refund(float cost);

    void AddMoney(float value);
}
