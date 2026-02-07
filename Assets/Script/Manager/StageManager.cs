using System;
using System.Collections;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    [SerializeField] private StageConfig stageConfig;
    [SerializeField] private EnemySpawner enemySpawner;

    private int currentWaveIndex = 0;
    private bool isRunning = false;
    private Coroutine stageRoutine;

    public int CurrentWave => currentWaveIndex + 1;
    public int MaxWave => stageConfig.waves.Length;

    public event Action WaveChanged;

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
            currentWaveIndex = i;
            Debug.Log($"wave: {CurrentWave} 開始");
            WaveChanged?.Invoke();
            // こちらが終わったら、またこのfor文が回ってWaveが動く。
            yield return StartCoroutine(RunWave(wave));
        }

        // ステージ終了処理
        isRunning = false;

        // 終わっても、敵はまだ画面にいるのでそのあたりを正しくチェック
        // クリアなどのイベントを作って通知...

    }

    private IEnumerator RunWave(WaveConfig wave)
    {
        // ボスウェーブのとき、なにかするならここで
        if (wave.isBossWave)
        {
            Debug.Log("boss!");
        }

        // 開始前待機時間
        yield return new WaitForSeconds(wave.startDelay);

        foreach (var group in wave.enemyGroups)
        {
            if (!isRunning)
                yield break;

            yield return StartCoroutine(SpawnGroup(group));

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

}
