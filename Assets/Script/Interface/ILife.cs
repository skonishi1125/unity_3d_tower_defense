using System;

public interface ILife
{
    int CurrentLife { get; }
    event Action<int> LifeChanged;
    void Damage(int amount);
    void Heal(int amount);

}
