using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    Edit,
    Playing,
    Slowing,
    Result
}

public class StateManager : MonoBehaviour
{
    public GameState State { get; private set; }
    [SerializeField] private LifeManager lifeManager;
    [SerializeField] private GameInput gameInput;

    [Header("Flags")]
    private bool isGameClear = false;

    [Header("UIs")]
    [SerializeField] private GameObject gameOverUI;
    public float elapsedTime { get; private set; }
    public event Action StateChanged;
    public event Action OnGameOver;
    public event Action OnGameClear;

    private void Awake()
    {
        if (lifeManager == null)
            lifeManager = FindFirstObjectByType<LifeManager>();

        if (gameInput == null)
            gameInput = FindFirstObjectByType<GameInput>();

        SetUpGame();
    }

    private void Update()
    {
        if (State == GameState.Playing)
            elapsedTime = Time.time;
    }

    private void OnEnable()
    {
        if (lifeManager != null)
            lifeManager.LifeZero += GameOver;

        if (gameInput != null)
            gameInput.ToggleModeRequested += ToggleModePressed;

        // リトライボタンの押された処理をstaticで管理している
        // これで、リトライボタンが画面にいくつもあっても、
        // どれか押されたときにReloadSceneを走らせるという設計にできている。
        RetryButton.OnRetryRequested += ReloadScene;

    }

    // リトライ処理などで、現状シーンを再読込する
    private void ReloadScene()
    {
        var scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

    private void OnDisable()
    {
        if (lifeManager != null)
            lifeManager.LifeZero -= GameOver;

        if (gameInput != null)
            gameInput.ToggleModeRequested -= ToggleModePressed;
    }

    private void SetUpGame()
    {
        // ゲーム開始時はEditモードから
        State = GameState.Edit;
        Time.timeScale = 0f;
        StateChanged?.Invoke();

        // GameOverUIが表示されていたら、非公開にする
        if (gameOverUI != null)
            gameOverUI.SetActive(false);

    }


    private void ToggleModePressed()
    {
        if (State == GameState.Playing)
        {
            gameInput.SetModeEdit(true);
            State = GameState.Edit;
            Time.timeScale = 0f;
        }
        else if (State == GameState.Edit)
        {
            gameInput.SetModeEdit(false);
            State = GameState.Playing;
            Time.timeScale = 1f;
        }

        StateChanged?.Invoke();
    }

    public void GameOver()
    {
        OnGameOver?.Invoke();
        StartCoroutine(GameOverSequence());
    }

    // SlowMotionCoが終わるまで待機するためのメソッド
    // Coroutineを直列化して、Coroutineの完了を待てるようにする
    private IEnumerator GameOverSequence()
    {
        yield return StartCoroutine(SlowMotionCo(false));

        // スロー終了後に以下が実行される
        EndGame();
    }

    public void GameClear()
    {
        OnGameClear?.Invoke();
        StartCoroutine(GameClearSequence());
    }

    private IEnumerator GameClearSequence()
    {
        yield return StartCoroutine(SlowMotionCo(false));

        // スロー終了後に以下が実行される
        Debug.Log("GAME CLEAR!");
        EndGame();
    }

    // スロー演出を入れて、クリア / ゲームオーバーと別の関数に移る。
    private IEnumerator SlowMotionCo(bool isGameClear)
    {
        State = GameState.Slowing;
        StateChanged?.Invoke();
        //AudioManager.Instance?.StopBgm();
        Time.timeScale = 0.5f;
        yield return new WaitForSecondsRealtime(3f);
        // Time.timeScale = 1f;
        // 0にしちゃってもいいかも。1にすると、リザルト画面放置してるとどんどんゴールされて不具合につながりそう。
        Time.timeScale = 0f;
    }

    private void EndGame()
    {
        State = GameState.Result;
        StateChanged?.Invoke();

        if (isGameClear)
        {
            // クリアの処理
        }
        else
        {
            // ゲームオーバーUIの表示
            if (gameOverUI != null)
                gameOverUI.SetActive(true);

        }
    }

}
