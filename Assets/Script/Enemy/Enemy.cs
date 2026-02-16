using System;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private EnemyMovement movement;
    private EnemyHealth health;

    public float LifeTime = 0f; // インスペクタで見るためにpublicとしておく

    // 進んだ距離
    public float TraveledDistance
    {
        get
        {
            if (movement == null)
                return 0f;
            return movement.GetTraveledDistance();
        }
    }

    // 外部に消滅したことを知らせるイベント
    public event Action<Enemy> OnDespawned;

    private void Awake()
    {
        movement = GetComponent<EnemyMovement>();
        health = GetComponent<EnemyHealth>();
    }

    private void Start()
    {
        LifeTime = 0f;
        if (movement != null)
            movement.ReachedGoal += OnReachedGoal;

        if (health != null)
            health.OnDied += OnDied;
    }

    private void Update()
    {
        LifeTime += Time.deltaTime;
        // 距離 = 速さ * 時間
        //TraveledDistance += (status.GetSpeed() * Time.deltaTime);
    }

    private void OnDestroy()
    {
        if (movement != null)
            movement.ReachedGoal -= OnReachedGoal;

        if (health != null)
            health.OnDied -= OnDied;
    }

    private void OnReachedGoal()
    {
        HandleDespawn();
    }

    private void OnDied()
    {
        HandleDespawn();
    }

    // 消滅時の共通処理
    private void HandleDespawn()
    {
        // StageManagerに「私が消滅します」と通知
        OnDespawned?.Invoke(this);

        Destroy(gameObject);
    }

}
