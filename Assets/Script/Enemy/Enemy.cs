using UnityEngine;

public class Enemy : MonoBehaviour
{
    private EnemyMovement movement;

    private void Awake()
    {
        movement = GetComponent<EnemyMovement>();
    }

    private void Start()
    {
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
