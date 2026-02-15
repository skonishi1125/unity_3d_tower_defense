using System;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    private EnemyStatus status;
    private EnemyMovement movement;
    private EnemyVfx vfx;
    private Slider healthBar;

    [SerializeField] protected float currentHp;
    [SerializeField] protected bool isDead;

    // 金額データを流すためのもの
    [Header("Broadcasting Events")]
    [SerializeField] private IntEventChannelSO onEnemyDiedChannel;

    public float CurrentHp => currentHp;
    public float MaxHp { get
        {
            if (status == null)
            {
                Debug.LogWarning("enemyHealth: MaxHpが取得できませんでした");
                return 0;
            }
            return status.GetMaxHp();
        }
    }

    public event Action OnTakeDamaged; // UI更新, 被弾音など必要なら購読する

    private void Awake()
    {
        status = GetComponent<EnemyStatus>();
        movement = GetComponent<EnemyMovement>();
        vfx = GetComponent<EnemyVfx>();
        healthBar = GetComponentInChildren<Slider>();

        currentHp = status.GetMaxHp();
        UpdateHealthBar();
    }

    private void UpdateHealthBar()
    {
        if (healthBar == null)
            return;

        healthBar.value = currentHp / status.GetMaxHp();
    }

    public void TakeDamage(float damage, float kbPower = 0f, float kbDuration = 0f)
    {
        ReduceHp(damage);

        // vfxのメソッドを直接読んでいる
        // HealthとVFXが密結合ではあるが、1つのEnemyというPrefab内で完結しているので構わない
        // 逆にPrefab内部で完結しないものはイベントとして購読させると良い。
        vfx?.PlayOnDamageVfx();
        vfx?.CreateDamagePopup(damage);
        OnTakeDamaged?.Invoke();

        // KB処理
        if (kbPower > 0f && movement != null)
            movement.ApplyKnockback(kbPower, kbDuration);
    }

    private void ReduceHp(float damage)
    {
        currentHp -= damage;
        UpdateHealthBar();

        if (currentHp <= 0)
            Die();
    }

    private void Die()
    {
        isDead = true;
        if (onEnemyDiedChannel != null)
            onEnemyDiedChannel.RaiseEvent(status.GetMoney());

        Destroy(gameObject);
    }

}
