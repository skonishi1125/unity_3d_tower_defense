using DG.Tweening;
using System;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    private EnemyStatus status;

    private Waypoint path;
    // WayPointに到達したと見なす範囲の猶予
    [SerializeField] private float arriveDistance = .1f;
    private Quaternion targetRotation;
    private float rotationSpeed = 10f;

    // 右から左に動くゲーム想定なので、最初は必ず左に動くという意味合いにしておく
    private Vector3 firstDirection = Vector3.left;
    public int CurrentIndex;

    private bool isKnockedBack = false;

    // 距離
    private float[] distanceToGoalCache; // 各種waypointからゴールまで
    private float totalPathDistance; // スポーン地点からゴールまでの総合距離

    public event Action ReachedGoal;

    public void Awake()
    {
        status = GetComponent<EnemyStatus>();
    }

    public void Initialize(Waypoint waypoint)
    {
        path = waypoint;
        CurrentIndex = 0;

        // ゴールから、各種Waypointごとの距離を計算しておく
        CalculatePathDistances();

        // 初回の方向決定
        DetectTargetRotate(firstDirection);
        transform.rotation = Quaternion.LookRotation(firstDirection, Vector3.up);

    }

    // ゴールから、逆順に距離を足していく
    // A -> B -> C -> ゴール の場合、
    // * C: dist(c ~ ゴール) までが距離
    // * B: dist(b ~ c) + dist(c ~ ゴール)
    // * A: dist(a ~ b) dist(b ~ c) + dist(c ~ ゴール) という感じ
    private void CalculatePathDistances()
    {
        if (path == null || path.Count == 0) return;

        int count = path.Count;
        distanceToGoalCache = new float[count];
        float accumulatedDist = 0f; // 累計距離

        // A B C D E F という配列のケース
        // count = Waypointの数そのもの。 indexとは1ずつずれるので、count 4 = waypoint[3]を指す
        // i = 6 - 2 = waypoint[4] (E) からスタート
        // i = 4 (E) : E - F の距離測定。累計距離追加 distanceToGoalCache[4] 格納
        // i = 3 (D) : D - E の距離測定。累計距離追加 distanceToGoalCache[3] 格納
        // i = 2 (C) : C - D の距離測定。累計距離追加 distanceToGoalCache[2] 格納
        // i = 1 (B) : B - C の距離測定。累計距離追加 distanceToGoalCache[1] 格納
        // i = 0 (A) : A - B の距離測定。累計距離追加 distanceToGoalCache[0] 格納
        for (int i = count - 2; i >= 0; i--)
        {
            float dist = Vector3.Distance(path.Get(i).position, path.Get(i + 1).position);
            accumulatedDist += dist;
            distanceToGoalCache[i] = accumulatedDist;
        }

        // 今回の例だと、 D の残り距離。 D = ゴールなので、0を入れておく
        if (count > 0)
            distanceToGoalCache[count - 1] = 0f;

        // スタート地点から最初のWaypointまでの距離を足す
        // 今回のゲーム設計だと最初のWaypoint = ほぼスタート地点なのであまり考えなくてよい
        // 一応書いておく（ 正確にはSpawn地点とWaypoint[0]の距離を出す必要があるが、ある程度ざっくりでよい ）
        float distToFirst = Vector3.Distance(transform.position, path.Get(0).position);
        totalPathDistance = distToFirst + distanceToGoalCache[0];
    }

    // 現在地からゴールまでの残り距離を返す
    public float GetRemainingDistance()
    {
        if (path == null || distanceToGoalCache == null) return 0f;
        if (CurrentIndex >= path.Count) return 0f; // ゴール済

        // 直近のWayPointまでの距離 例えばゲーム開始直後なら、
        // Waypoint A - B の距離
        float distToNextWp = Vector3.Distance(transform.position, path.Get(CurrentIndex).position);

        // 例えばゲーム開始直後なら、
        // A - B の距離 + distanceToGoalCache[1] (B から最後のWaypointまでの距離)
        return distToNextWp + distanceToGoalCache[CurrentIndex];
    }

    // 全長 - 残り距離 = 進んだ距離を返す
    public float GetTraveledDistance()
    {
        return totalPathDistance - GetRemainingDistance();
    }

    private void Update()
    {
        // パス未指定
        // もしくは、被弾してノックバック中は動かない
        if (path == null || path.Count == 0 || isKnockedBack)
            return;

        Transform target = path.Get(CurrentIndex);
        if (target == null)
            return;

        // 敵(A)とwaypoint(B)の向きベクトルの取得。 AがBに向かうので、 B - A
        Vector3 to = target.position - transform.position;
        to.y = 0f; // y軸の高さは考慮させない（平行移動させる）

        //Debug.Log($"idx={currentIndex} target={target.name} targetPos={target.position} enemyPos={transform.position}");
        //Debug.DrawLine(transform.position, target.position, Color.yellow);

        // Waypointに到達したときの判定
        // sqr: square(2乗)を意味する単語 magnitude: 大きさ、絶対値を表す単語
        // つまり、(B-A)^2 の大きさが、(有効距離)^2よりも少なければ到達したとみなしている
        // to.magnitude = √ が考慮されるので、2乗して処理を軽めにしている
        float sqrDist = to.sqrMagnitude;

        if (sqrDist <= arriveDistance * arriveDistance)
        {
            CurrentIndex++;
            if (CurrentIndex >= path.Count)
            {
                ReachedGoal?.Invoke();
                enabled = false;
                return;
            }

            // 目的地更新
            Transform newTarget = path.Get(CurrentIndex);
            Vector3 newTo = newTarget.position - transform.position;
            to.y = 0f;
            DetectTargetRotate(newTo.normalized);

        }

        Move(to.normalized);
        RotateSmoothly();


        //if (Input.GetKeyDown(KeyCode.A))
        //    ApplyKnockback(1f);


    }

    private void Move(Vector3 dir)
    {
        float moveSpeed = 0f;
        if (status != null)
            moveSpeed = status.GetSpeed();
        transform.position += dir * moveSpeed * Time.deltaTime;
    }

    private void DetectTargetRotate(Vector3 dir)
    {
        if (dir != Vector3.zero)
            targetRotation = Quaternion.LookRotation(dir);

        // 進行方向にRayを出して可視化 デバッグ用
        //Debug.DrawRay(transform.position, transform.forward * 2f, Color.blue);
        //Debug.DrawRay(transform.position, dir * 2f, Color.red);
    }
    private void RotateSmoothly()
    {
        // 即時振り向きの場合は下記
        // transform.rotation = Quaternion.LookRotation(dir, Vector3.up);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            Time.deltaTime * rotationSpeed
        );
    }

    public void ApplyKnockback(float knockbackDistance)
    {
        isKnockedBack = true;

        Vector3 knockbackDir = -transform.forward;

        transform.DOMove(knockbackDir * knockbackDistance, 0.05f)
            // 相対座標移動 今の位置から、進行方向とは逆の方向に下がらせる
            .SetRelative(true)
            .OnComplete(() =>
            {
                isKnockedBack = false;
            });
    }


}
