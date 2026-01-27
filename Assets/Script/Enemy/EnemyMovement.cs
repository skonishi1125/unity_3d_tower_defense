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
    private int previewIndex;

    // 右から左に動く想定なので、最初は必ず左に動くという意味合いにしておく
    public Vector3 FirstDirection = Vector3.left;
    public int CurrentIndex;

    public event Action ReachedGoal;

    public void Awake()
    {
        status = GetComponent<EnemyStatus>();

        // 初回の方向決定
        DetectTargetRotate(FirstDirection);
    }

    public void Initialize(Waypoint waypoint)
    {
        path = waypoint;
        CurrentIndex = 0;
        previewIndex = 0;

        // 初回の方向決定
        //Transform target = path.Get(CurrentIndex);
        //Vector3 to = target.position - transform.position;
        //to.y = 0f;
        //DetectTargetRotate(to.normalized);

    }

    private void Update()
    {
        if (path == null || path.Count == 0)
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
        }

        // 新しい行き先(Waypoint)へと更新されたときの、目的地変更処理
        // 1フレーム誤差があるが、気にならない程度なので一旦このまま
        // （問題あれば、CurrentIndex++としたのち、新しいWaypointを取得してそこで更新すれば良い）
        if (previewIndex != CurrentIndex)
        {
            DetectTargetRotate(to.normalized);
            previewIndex = CurrentIndex;
        }

        Move(to.normalized);
        DetectTargetRotate(to.normalized);

        RotateSmoothly();
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
}
