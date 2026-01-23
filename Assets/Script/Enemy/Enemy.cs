using UnityEngine;

public class Enemy : MonoBehaviour
{
    private EnemyMovement movement;
    private EnemyStatus status;

    public float LifeTime = 0f; // インスペクタで見るためにpublicとしておく
    public float TraveledDistance = 0f;

    private void Awake()
    {
        movement = GetComponent<EnemyMovement>();
        status = GetComponent<EnemyStatus>();
    }

    private void Start()
    {
        LifeTime = 0f;
        TraveledDistance = 0f;
        movement.ReachedGoal += OnReachedGoal;
    }

    private void Update()
    {
        LifeTime += Time.deltaTime;
        // 距離 = 速さ * 時間
        TraveledDistance += (status.GetSpeed() * Time.deltaTime);
    }

    private void OnDestroy()
    {
        if (movement != null)
            movement.ReachedGoal -= OnReachedGoal;
    }

    private void OnReachedGoal()
    {
        Debug.Log("Enemy reached goal");
        Destroy(gameObject);
    }

}
