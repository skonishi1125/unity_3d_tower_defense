using System;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    private EnemyStatus status;
    [SerializeField] private float arriveDistance = .1f;

    private Waypoint path;
    public int CurrentIndex;

    public event Action ReachedGoal;

    public void Awake()
    {
        status = GetComponent<EnemyStatus>();
    }

    public void Initialize(Waypoint waypoint)
    {
        Debug.Log(waypoint);
        path = waypoint;
        CurrentIndex = 0;

        Debug.Log($"Initialized! path:{path} CurrentIndex: {CurrentIndex}");

        // デフォルトポジションがあるなら、ここに書く

    }

    private void Update()
    {
        if (path == null || path.Count == 0)
            return;

        Transform target = path.get(CurrentIndex);
        if (target == null)
            return;

        // 敵(A)とwaypoint(B)の向きベクトルの取得。 AがBに向かうので、 B - A
        Vector3 to = target.position - transform.position;
        to.y = 0f; // y軸の高さは考慮させない（平行移動させる）

        //Debug.Log($"idx={currentIndex} target={target.name} targetPos={target.position} enemyPos={transform.position}");
        //Debug.DrawLine(transform.position, target.position, Color.yellow);

        // sqr: square(2乗)を意味する単語 magnitude: 大きさ、絶対値を表す単語
        // つまり、(B-A)^2 の大きさが、(有効距離)^2よりも少なければ到達したとみなしている
        // to.magnitude = √ が考慮されるので、2乗して処理を軽めにしている
        float sqrDist = to.sqrMagnitude;

        if (sqrDist <= arriveDistance * arriveDistance)
        {
            //Debug.Log($"{sqrDist}, {arriveDistance * arriveDistance} ");
            CurrentIndex++;

            if (CurrentIndex >= path.Count)
            {
                ReachedGoal?.Invoke();
                enabled = false;
            }
            return;
        }

        // 移動
        Vector3 dir = to.normalized;
        float moveSpeed = 0f;
        if (status != null)
            moveSpeed = status.GetSpeed();
        transform.position += dir * moveSpeed * Time.deltaTime;

    }


}
