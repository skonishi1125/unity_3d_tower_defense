using Unity.VisualScripting;
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

        // この形で呼び出すと、spawnPointのGameObjectの子要素として作られてしまう
        // Instantiate(SpawnEnemies[0], spawnPoint);
        // 生成場所を指定したい場合は、以下のようにposition, rotationどちらも明示的に指定する必要がある
        var go = Instantiate(SpawnEnemies[0], spawnPoint.position, spawnPoint.rotation);
        // ちなみに第4引数にTransform parent などとすると、その親の子要素として位置も指定しつつ生成ができる
        var mover = go.GetComponent<EnemyMovement>();
        mover.Initialize(waypoint);

        timer = duration;
        spawnCount++;
        Debug.Log("spawn!");


    }
}
