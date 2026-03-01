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

    public int GetIntValue()
    {
        // (int)とせず、四捨五入の形で返す
        // ↑だと、99.9999f 等の場合、99fが返る
        return Mathf.RoundToInt(GetValue());
    }

    public void AddBonus(float v)
    {
        additiveBonus += v;
    }
    public void SetBonus(float v)
    {
        additiveBonus = v;
    }

    public void AddMultiplier(float v)
    {
        multiplier += v;
    }

    public void SetMultiplier(float v)
    {
        multiplier = v;
    }

    public void SetBaseValue(float v)
    {
        baseValue = v;
    }


}
