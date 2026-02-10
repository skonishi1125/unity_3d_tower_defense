using UnityEngine;

public class Enemy : MonoBehaviour
{
    private EnemyMovement movement;

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

    private void Awake()
    {
        movement = GetComponent<EnemyMovement>();
    }

    private void Start()
    {
        LifeTime = 0f;
        //TraveledDistance = 0f;
        movement.ReachedGoal += OnReachedGoal;
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
    }

    private void OnReachedGoal()
    {
        //Debug.Log("Enemy reached goal");
        Destroy(gameObject);
    }

}
