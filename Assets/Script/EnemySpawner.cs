using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private Waypoint waypoint;
    private Transform spawnTransform;

    private void Start()
    {
        // 初期スポーン設定
        if (waypoint == null)
            waypoint = FindFirstObjectByType<Waypoint>();

        // 最初のwaypoint = スポーン位置
        spawnTransform = waypoint.Get(0);
    }


    public void Spawn(GameObject enemyPrefab)
    {
        var go = Instantiate(enemyPrefab, spawnTransform.position, Quaternion.identity);
        // 敵のスポーンy座標 = (Colliderの高さの半分) + (床の厚み * .5f)
        float spawnHeight = 0f;
        var col = go.GetComponent<Collider>();
        if (col != null)
            spawnHeight = col.bounds.extents.y;

        spawnHeight += RoadGenerator.RoadThickness * .5f;
        go.transform.position = spawnTransform.position + Vector3.up * spawnHeight;

        // 移動, rotate初期化
        var mover = go.GetComponent<EnemyMovement>();
        mover.Initialize(waypoint);

    }

    //private void Spawn(Transform spawnPoint)
    //{
    //    // Instantiate(SpawnEnemies[0], spawnPoint);
    //    // ↑この形で呼び出すと、spawnPointのGameObjectの子要素として作られてしまう
    //    // 生成場所を指定したい場合は、以下のようにposition, rotationどちらも明示的に指定する必要がある
    //    // ちなみに第4引数にTransform parent などとすると、その親の子要素として位置も指定しつつ生成ができる
    //    var go = Instantiate(SpawnEnemies[0], spawnPoint.position, Quaternion.identity);

    //    // 敵のスポーンy座標 = (Colliderの高さの半分) + (床の厚み * .5f)
    //    float spawnHeight = 0f;
    //    var col = go.GetComponent<Collider>();
    //    if (col != null)
    //        spawnHeight = col.bounds.extents.y;

    //    spawnHeight += RoadGenerator.RoadThickness * .5f;
    //    go.transform.position = spawnPoint.position + Vector3.up * spawnHeight;

    //    // 移動, rotate初期化
    //    var mover = go.GetComponent<EnemyMovement>();
    //    mover.Initialize(waypoint);

    //    // タイマー設定
    //    timer = duration;
    //    spawnCount++;
    //}
}
