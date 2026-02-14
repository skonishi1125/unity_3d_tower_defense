using UnityEngine;

public class TowerStatus : MonoBehaviour
{
    [SerializeField] private Status maxHp; // 0になると壊れる
    [SerializeField] private Status attack;
    [SerializeField] private Status attackInterval; // 攻撃にかかる時間
    [SerializeField] private Status attackRange; // スカラー 攻撃する距離 3 = 3マス分にしたい
    [SerializeField] private Status viewingAngle; // 視野角 90° = ±45°の方向で広がることになる
    [SerializeField] private Status cost; // 建造費用
    [SerializeField] private Status knockbackPower;
    [SerializeField] private Status knockbackDuration;

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

    public float GetCost()
    {
        return cost.GetValue();
    }

    public float GetKnockbackPower()
    {
        return knockbackPower.GetValue();
    }

    public float GetKnockbackDuration()
    {
        return knockbackDuration.GetValue();
    }

    // TowerCombatとか別箇所に書くと、Playモード中以外は取得できずにエラーになる
    // (status.Get()...という形になるが、statusはAwakeしないと読まないので）
    private void OnDrawGizmos()
    {
        // 索敵範囲
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, GetAttackRange());

        // 視野
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, transform.forward * GetAttackRange()); // 正面

        float halfAngle = GetViewingAngle() * .5f;
        Quaternion leftRayRotation = Quaternion.AngleAxis(-halfAngle, Vector3.up);
        Quaternion rightRayRotation = Quaternion.AngleAxis(halfAngle, Vector3.up);

        Vector3 leftRayDirection = leftRayRotation * transform.forward;
        Vector3 rightRayDirection = rightRayRotation * transform.forward;

        Gizmos.DrawRay(transform.position, leftRayDirection * GetAttackRange());
        Gizmos.DrawRay(transform.position, rightRayDirection * GetAttackRange());

    }



}
