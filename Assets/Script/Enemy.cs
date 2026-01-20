using UnityEngine;

public class Enemy : MonoBehaviour
{
    private EnemyMovement movement;
    [SerializeField] private Waypoint waypoint;

    private void Awake()
    {
        movement = GetComponent<EnemyMovement>();
    }

    private void Start()
    {
        movement.Initialize(waypoint);
        movement.ReachedGoal += OnReachedGoal;
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
