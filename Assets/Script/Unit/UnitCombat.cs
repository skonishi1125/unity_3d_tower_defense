using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UnitCombat : MonoBehaviour
{
    private Unit unit;
    private UnitStatus status;

    [Header("Attack")]
    private float attackTimer;
    private Tween attackTween;
    [SerializeField] private Slider coolTimeBar;

    [Header("Attack Hop")]
    [SerializeField] private float hopHeight = 1f;
    [SerializeField] private float hopDuration = 0.1f;

    [Header("Detection")]
    [SerializeField] private LayerMask whatIsEnemy;

    private void Awake()
    {
        unit = GetComponent<Unit>();
        status = GetComponent<UnitStatus>();
        if (coolTimeBar == null)
            coolTimeBar = GetComponentInChildren<Slider>();
    }

    private void Start()
    {
        UpdateCoolTimeBar();
    }

    private void Update()
    {
        // ゲーム内時間が止まっているときは、処理を走らせない
        // (Editモード中などは、敵を殴らせない）
        if (Time.deltaTime == 0f)
            return;

        // Ghost中などは、タイマーを走らせない
        if (unit.CurrentUnitState != UnitState.Battle)
            return;

        attackTimer -= Time.deltaTime;
        UpdateCoolTimeBar();

        // タイマーをこの時点でリセットすると、
        // 経過したとき誰もいなかったらクールタイムがリセットされる
        // なので、攻撃が成功した時にタイマーはリセットされるようにする。
        if (TryGetTarget(out var targetEnemyHealth))
            PerformAttack(targetEnemyHealth);
    }

    // クールタイムを示すSliderバーの更新処理
    private void UpdateCoolTimeBar()
    {
        // attackTimerが0に近づくほど、Sliderのvalueは1に近づく
        // ex) 5秒おきの攻撃の場合
        // * attackTimer = 5 -> (1 - (5/5)) = 0
        // * attackTimer = 4 -> (1 - (4/5)) = 0.2
        // ...
        // * attackTimer = 1 -> (1 - (1/5)) = 0.8
        // * attackTimer = 0 -> (1 - (0/5)) = 1 ※攻撃準備完了
        float calculateValue = 1 - (attackTimer / status.GetAttackInterval());
        if (calculateValue < 0)
            calculateValue = 0;

        coolTimeBar.value = calculateValue;
    }

    private void PerformAttack(EnemyHealth h)
    {

        attackTween?.Kill();

        // 攻撃時に跳ねる処理
        attackTween = transform
            .DOJump(transform.position, hopHeight, 1, hopDuration)
            .SetEase(Ease.OutQuad)
            .SetLink(gameObject, LinkBehaviour.KillOnDestroy);

        // ダメージ処理
        float damage = status.GetAttack();
        float kbPower = status.GetKnockbackPower();
        float kbDuration = status.GetKnockbackDuration();

        h.TakeDamage(damage, kbPower, kbDuration);

        // 攻撃が終わったら、インターバルリセット
        attackTimer = status.GetAttackInterval();
    }

    protected virtual bool TryGetTarget(out EnemyHealth attackEnemyHealth)
    {
        attackEnemyHealth = null;

        if (attackTimer > 0f)
            return false;

        // 攻撃距離でOverlapSphereを実行して、周囲の敵を取得
        float range = status.GetAttackRange();
        Collider[] hitEnemies = Physics.OverlapSphere(transform.position, range, whatIsEnemy);
        if (hitEnemies == null || hitEnemies.Length == 0) return false;

        // 取得した敵を、以下の条件で振り分ける
        // * 視野角内かどうか
        // * 視野角内であれば、最も進んでいる敵を攻撃
        Vector3 unitPos = transform.position;
        Vector3 forwardDir = transform.forward;

        // 敵判定に使用する得点
        // * 通常: 最も移動した敵（移動すればするほどスコアが上がる）
        // * Weaker: 最も体力の割合が少ない敵　（少ないほどスコアを上げる）
        float targetScore = 0f;

        foreach (var hit in hitEnemies)
        {
            Vector3 enemyPos = hit.transform.position;
            Vector3 targetDir = (enemyPos - unitPos).normalized;

            // Unitから見た正面ベクトル と 敵ポジションを内積で比較
            float dot = Vector3.Dot(forwardDir, targetDir);

            // GetViewingAngle()は、Degreeで返る。
            // また合計の値が返る（90°なら、前方から-45から+45°の範囲となる）。
            // ex) 90°なら、radianにして、cos45° (0.707...)を返して、内積と比較させる
            float halfAngleRad = (status.GetViewingAngle() * .5f) * Mathf.Deg2Rad;
            float cos = Mathf.Cos(halfAngleRad);

            //Debug.Log($"内積: {dot} cosθ: {cos}");

            if (dot > cos)
            {
                // colliderは敵のVisualに付与されているので、
                // Scriptデータを取りたいなら、Parant側から取る必要がある
                var enemy = hit.GetComponentInParent<Enemy>();
                if (enemy == null)
                    continue;
                var enemyHealth = hit.GetComponentInParent<EnemyHealth>();
                if (enemyHealth == null || enemyHealth.IsDead)
                    continue;

                // 複数の敵から、攻撃対象を特定する
                DetectAttackTarget(enemy, enemyHealth, ref attackEnemyHealth, ref targetScore);
            }

        }

        return attackEnemyHealth != null;
    }

    // 敵が複数検出された場合の、攻撃対象選択処理
    protected virtual void DetectAttackTarget(Enemy enemy, EnemyHealth enemyHealth, ref EnemyHealth attackEnemyHealth, ref float score)
    {
        // 最も移動している敵をattackEnemyHealthに格納する
        if (attackEnemyHealth == null || score < enemy.TraveledDistance)
        {
            attackEnemyHealth = enemyHealth;
            score = enemy.TraveledDistance;
        }
    }

}
