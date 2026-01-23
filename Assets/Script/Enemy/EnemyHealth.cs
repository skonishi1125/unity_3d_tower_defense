using System;
using UnityEngine;

// TODO: ヘルスバーを用意しておく
public class EnemyHealth : MonoBehaviour
{
    private Enemy enemy;
    private EnemyStatus status;
    private EnemyVfx vfx;

    [SerializeField] protected float currentHp;
    [SerializeField] protected bool isDead;

    // HealthBar更新や被弾音（必要なら購読する）
    public event Action OnTakeDamaged;

    private void Awake()
    {
        enemy = GetComponent<Enemy>();
        status = GetComponent<EnemyStatus>();
        vfx = GetComponent<EnemyVfx>();

        currentHp = status.GetMaxHp();
    }

    public void TakeDamage(float damage)
    {
        ReduceHp(damage);
        vfx?.PlayOnDamageVfx(); // イベントでも良いかもしれない。
        OnTakeDamaged?.Invoke();
    }

    private void ReduceHp(float damage)
    {
        currentHp -= damage;

        if (currentHp <= 0)
            Die();
    }

    private void Die()
    {
        isDead = true;
        Destroy(gameObject);
        Debug.Log("died!");
        // スコアを入れたり、お金を増やしたり
    }

}
