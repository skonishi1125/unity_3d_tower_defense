using Unity.VisualScripting;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private Waypoint waypoint;
    [SerializeField] private GameObject[] SpawnEnemies;
    [SerializeField] private int maxSpawnCount = 20;
    [SerializeField] private float duration = 1.0f;
    private float timer;

    private int currentIndex = 0;
    private int spawnCount = 0;

    private void Start()
    {
        // 初期スポーン設定
        //timer = duration;
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

        // Instantiate(SpawnEnemies[0], spawnPoint);
        // ↑この形で呼び出すと、spawnPointのGameObjectの子要素として作られてしまう
        // 生成場所を指定したい場合は、以下のようにposition, rotationどちらも明示的に指定する必要がある
        // ちなみに第4引数にTransform parent などとすると、その親の子要素として位置も指定しつつ生成ができる
        //var go = Instantiate(SpawnEnemies[0], spawnPoint.position, spawnPoint.rotation);

        Vector3 spawnPos = spawnPoint.position;
        // 敵のスポーンy座標 = (scale * .25f) + (床の厚み * .5f)
        //spawnPos.y = (SpawnEnemies[0].transform.localScale.y * .25f) + (RoadGenerator.RoadThickness * .5f);
        //Debug.Log(spawnPos.y);
        var go = Instantiate(SpawnEnemies[0], spawnPos, spawnPoint.rotation);
        float spawnHeight = 0f;
        var col = go.GetComponent<Collider>();
        if (col != null)
            spawnHeight = col.bounds.extents.y;

        Debug.Log(spawnHeight);
        spawnHeight += RoadGenerator.RoadThickness * .5f;
        go.transform.position = spawnPoint.position + Vector3.up * spawnHeight;

        var mover = go.GetComponent<EnemyMovement>();
        mover.Initialize(waypoint);

        timer = duration;
        spawnCount++;
    }
}
