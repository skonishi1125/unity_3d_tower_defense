using UnityEngine;
using DG.Tweening;

public class TowerCombat : MonoBehaviour
{
    private TowerStatus status;

    [Header("Attack")]
    private float attackTimer;
    private float attackInterval = 1f; // TODO: TowerStatusから参照する
    private Tween attackTween;

    [Header("Attack Hop")]
    [SerializeField] private float hopHeight = 1f;
    [SerializeField] private float hopDuration = 0.1f;

    [Header("Detection")]
    [SerializeField] private float range = 3f;
    [SerializeField] private LayerMask whatIsEnemy;


    private void Awake()
    {
        status = GetComponent<TowerStatus>();
    }

    private void Update()
    {
        attackTimer -= Time.deltaTime;

        // タイマーをこの時点でリセットすると、タイマーが経過したとき誰もいなかったらクールタイムがリセットされる
        // なので、攻撃した時にタイマーはリセットされるようにする。
        if (TryGetTarget(out var target))
        {
            attackTween?.Kill();

            // 攻撃時に跳ねる処理
            attackTween = transform
                .DOJump(transform.position, hopHeight, 1, hopDuration)
                .SetEase(Ease.OutQuad)
                .SetLink(gameObject, LinkBehaviour.KillOnDestroy);
            target.TakeDamage(status.GetAttack());
            attackTimer = attackInterval;
        }
    }

    private bool TryGetTarget(out EnemyHealth best)
    {
        best = null;

        if (attackTimer > 0f)
            return false;

        Collider[] hits = Physics.OverlapSphere(transform.position, range, whatIsEnemy);
        if (hits == null || hits.Length == 0) return false;

        float bestSqrDist = float.PositiveInfinity;

        foreach (var hit in hits)
        {
            var enemy = hit.GetComponentInParent<EnemyHealth>();
            if (enemy == null) continue;

            float sqrDist = (enemy.transform.position - transform.position).sqrMagnitude;
            if (sqrDist < bestSqrDist)
            {
                bestSqrDist = sqrDist;
                best = enemy;
            }
        }

        return best != null;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, range);
    }




}
