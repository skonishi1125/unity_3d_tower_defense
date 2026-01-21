using UnityEngine;

public class TowerStatus : MonoBehaviour
{
    public Status maxHp; // 0になると壊れる
    public Status attack;
    [Range(0f, 1f)]
    public Status attackInterval; // 攻撃頻度 0.1秒など

    public float GetMaxHp()
    {
        return maxHp.GetValue();
    }

    public float GetAttack()
    {
        return attack.GetValue();
    }

    public float GetAttackInterval()
    {
        return attackInterval.GetValue();
    }

}
