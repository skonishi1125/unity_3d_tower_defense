using UnityEngine;

public class TowerCombat : MonoBehaviour
{
    private TowerStatus status;

    private bool isAttacking = false;
    // 敵のレイヤ whatIsEnemy

    private void Awake()
    {
        status = GetComponent<TowerStatus>();
    }

    private void Update()
    {
        if (isAttacking)
            PerformAttack();
    }

    private void PerformAttack()
    {
        isAttacking = true;

        // 前方にRayを撃ち、感知した敵を取得
        // 敵.GetComponent<EnemyHealth>()を取って、TakeDamage(TowerStatus.Attack)とする
        // (インターフェースとしてもよい)

        // TowerStatus.attackIntervalの分だけ待つ
        isAttacking = false;
    }




}
