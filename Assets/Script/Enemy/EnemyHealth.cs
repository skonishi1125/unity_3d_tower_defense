using System;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    private Enemy enemy;
    private EnemyStatus status;

    [SerializeField] protected float currentHp;
    [SerializeField] protected bool isDead;

    // HealthBar更新や被弾音（必要なら購読する）
    public event Action OnTakeDamaged;

    private void Awake()
    {
        enemy = GetComponent<Enemy>();
        status = GetComponent<EnemyStatus>();

        currentHp = status.GetMaxHp();
    }

    public void TakeDamage(float damage)
    {
        ReduceHp(damage);
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
        // スコアを入れたり、お金を増やしたり
    }

}
