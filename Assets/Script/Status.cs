using System;
using UnityEngine;

[Serializable]
public class Status
{
    [SerializeField] private float baseValue;
    [SerializeField] private float additiveBonus = 0;
    [SerializeField] private float multiplier = 1f;

    // バフなど含めた合計値がほしいときは、これを使えば良い
    public float GetValue()
    {
        return (baseValue + additiveBonus) * multiplier;
    }

    public void AddBonus(float v)
    {
        additiveBonus += v;
    }

    public void AddMultiplier(float v)
    {
        multiplier += v;
    }

    public void SetBaseValue(float v)
    {
        baseValue = v;
    }

}
