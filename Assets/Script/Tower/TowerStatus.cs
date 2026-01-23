using UnityEngine;

public class TowerStatus : MonoBehaviour
{
    public Status maxHp; // 0になると壊れる
    public Status attack;
    public Status attackInterval; // 攻撃頻度 0.1秒など
    public Status attackRange; // スカラー 攻撃する距離 3 = 3マス分にしたい
    public Status viewingAngle; // 視野角 45 = 実質90°で感知する

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
    public float GetAttackRange()
    {
        // 実際オブジェクトは0.5f分考慮した距離に配置されるので、
        // こうすることで実質3マス分の範囲として考えることができる
        return attackRange.GetValue() + .5f;
    }

    public float GetViewingAngle()
    {
        return viewingAngle.GetValue();
    }



}
