using System;
using UnityEngine;

public enum GameState
{
    Ready,
    WaveIntro, // 3 2 1...と、カウントダウン
    Playing,
    Slowing,
    Result
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    // ゲーム全体の状態
    public GameState State { get; private set; } = GameState.Playing;

    public float money { get; private set; }
    public float elapsedTime { get; private set; }
    public float currentLife { get; private set; }

    private void Awake()
    {
        // Scene遷移したとき、そのSceneにGameManagerがあったときの配慮
        // これでDebug時、各シーンにGamemanagerを置いておける
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        SetDefaultValue();

    }

    private void Update()
    {
        if (State == GameState.Playing)
            elapsedTime = Time.time;
    }

    private void SetDefaultValue()
    {
        money = 100;
        currentLife = 10;
    }

    public void IncreaseLife()
    {
        currentLife += 1;
    }

    public void DecreaseLife()
    {
        currentLife -= 1;
        if (currentLife == 0)
        {
            GameOver();
        }
    }

    public void GameOver()
    {
        // ゲームオーバー特有の処理をしたあと
        EndGame();
    }

    public void GameClear()
    {
        // クリア特有の処理をしたあと
        EndGame();
    }

    private void EndGame()
    {
        // クリア、ゲームオーバー共通処理
        // (もういちど、とか）
    }

}
