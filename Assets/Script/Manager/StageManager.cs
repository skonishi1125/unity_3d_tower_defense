using System;
using System.Collections;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    [SerializeField] private StageConfig stageConfig;
    [SerializeField] private EnemySpawner enemySpawner;
    [SerializeField] private BossAlert bossAlert;
    [SerializeField] private ClearFlash clearFlash;

    [Header("Stage Information")]
    private int currentWaveIndex = 0;
    private bool isRunning = false;
    private Coroutine stageRoutine;
    private int activeEnemyCount = 0;
    private bool isBossWaveSpawnComplete = false;

    public int CurrentWaveIndex => currentWaveIndex;
    public int MaxWave => stageConfig.waves.Length;

    public event Action WaveChanged;
    public event Action OnAllWavesCompleted; // 全てのWave呼出済み && 画面上に敵がいない

    private void Start()
    {
        BeginStage();
    }

    private void BeginStage()
    {
        if (isRunning)
            return;

        isRunning = true;
        stageRoutine = StartCoroutine(RunStage());
    }

    // ステージ全体
    private IEnumerator RunStage()
    {
        for (int i = 0; i < stageConfig.waves.Length; i++)
        {
            if (!isRunning)
                yield break;

            var wave = stageConfig.waves[currentWaveIndex];

            if (wave.isBossWave)
            {
                Debug.Log($"Boss Wave! wave: {CurrentWaveIndex + 1}");
                if (bossAlert != null)
                    bossAlert.Play();
            }
            else
            {
                Debug.Log($"wave: {CurrentWaveIndex + 1} 開始");
            }

            WaveChanged?.Invoke();
            // こちらが終わったら、またこのfor文が回ってWaveが動く。
            yield return StartCoroutine(RunWave(wave));
            currentWaveIndex++;
        }

        // ステージ終了処理
        isRunning = false;

        // 終わっても、敵はまだ画面にいるのでそのあたりを正しくチェック
        // クリアなどのイベントを作って通知...

    }

    private IEnumerator RunWave(WaveConfig wave)
    {
        // 開始前待機時間
        yield return new WaitForSeconds(wave.startDelay);

        foreach (var group in wave.enemyGroups)
        {
            if (!isRunning)
                yield break;

            yield return StartCoroutine(SpawnGroup(group));
        }

        if (wave.isBossWave)
        {
            isBossWaveSpawnComplete = true;
            CheckWaveCompletion();
        }

    }

    // 渡された敵グループをSpawnする
    // 敵ごとに沸く感覚時間などがあるので、Coroutineを分けている
    private IEnumerator SpawnGroup(EnemyGroup group)
    {
        for (int i = 0; i < group.spawnCount; i++)
        {
            if (!isRunning)
                yield break;

            var enemyPrefab = group.enemyPrefab;
            if (enemyPrefab == null)
            {
                Debug.LogWarning("StageManager:SpawnGroup: 敵Prefab情報が取得できませんでした。");
                yield break;
            }

            enemySpawner.Spawn(enemyPrefab);

            yield return new WaitForSeconds(group.spawnInterval);
        }
    }

    // スポナーで敵を生成したとき、
    // * こちらの関数でカウント加算
    // * イベントに購読し、敵が消えたときにカウント減算されるようにする
    public void RegisterEnemy(Enemy enemy)
    {
        activeEnemyCount++;
        enemy.OnDespawned += HandleEnemyDespawned;
    }

    private void HandleEnemyDespawned(Enemy enemy)
    {
        enemy.OnDespawned -= HandleEnemyDespawned;
        activeEnemyCount--;

        CheckWaveCompletion();
    }

    private void CheckWaveCompletion()
    {
        // ボスウェーブの全スポーンが終わり、かつ画面に敵がいない場合
        if (isBossWaveSpawnComplete && activeEnemyCount <= 0)
        {
            if (clearFlash != null)
                clearFlash.Play();
            OnAllWavesCompleted?.Invoke();
        }
    }

}
