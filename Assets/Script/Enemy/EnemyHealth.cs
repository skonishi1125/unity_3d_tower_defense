using System;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    private EnemyStatus status;
    private EnemyMovement movement;
    private EnemyVfx vfx;
    private EnemySfx sfx;
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
    public bool IsDead => isDead;

    public event Action OnTakeDamaged; // UI更新, 被弾音など必要なら購読する
    public event Action OnDied;

    private void Awake()
    {
        status = GetComponent<EnemyStatus>();
        movement = GetComponent<EnemyMovement>();
        vfx = GetComponent<EnemyVfx>();
        sfx = GetComponent<EnemySfx>();
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

    public void TakeDamage(float pureDamage, UnitStatus unitStatus, float kbPower = 0f, float kbDuration = 0f)
    {
        if (isDead) return;

        float damage = CalculateDamageAndCondition(pureDamage, unitStatus);
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

    // 防御力を考慮したダメージの計算
    // 0のダメージはなく、最低 1 を返す
    private float CalculateDamageAndCondition(float damage, UnitStatus unitStatus)
    {
        // 防御力取得
        float defense = 0f;
        if (status != null)
            defense = status.GetDefense();

        // Metal特攻持ち かつ Metal の場合、3ダメージとして返す
        if (unitStatus.IsEffectiveMetal() && status.IsMetal())
            return 3f;

        // Sky 特攻持ち かつ Sky の場合、ダメージを2倍にする
        if (unitStatus.IsEffectiveSky() && status.IsSky())
            damage = damage * 2f;

        // 鈍足デバフ持ちなら、敵を遅くする
        if (unitStatus.HasSpeedDown())
            status.speed.SetMultiplier(.5f);

        // ダメージ計算
        float real_damage = damage - defense;

        if (real_damage <= 1f)
            real_damage = 1f;

        return real_damage;

    }

    private void ReduceHp(float damage)
    {
        currentHp -= damage;
        if (sfx != null)
            sfx.PlayHitted();
        UpdateHealthBar();

        if (currentHp <= 0)
            Die();
    }

    private void Die()
    {
        if (isDead) return;

        isDead = true;
        if (onEnemyDiedChannel != null)
            onEnemyDiedChannel.RaiseEvent(status.GetMoney());

        vfx?.CreateMoneyPopup(status.GetMoney());

        OnDied?.Invoke();
        Destroy(gameObject);
    }

}
