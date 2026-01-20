using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private Waypoint waypoint;
    [SerializeField] private GameObject[] SpawnEnemies;
    private float timer;
    private float duration = 5.0f;
    private int maxSpawnCount = 5;

    private int currentIndex = 0;
    private int spawnCount = 0;

    private void Start()
    {
        // 初期スポーン設定
        timer = duration;
    }


    private void Update()
    {
        timer -= Time.deltaTime;
        if (spawnCount < maxSpawnCount)
            if (timer < 0f)
                Spawn(waypoint.get(currentIndex));
    }

    private void Spawn(Transform spawnPoint)
    {
        Instantiate(SpawnEnemies[0], spawnPoint);
        timer = duration;
        spawnCount++;
        Debug.Log("spawn!");


    }
}
