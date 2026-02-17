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
    [SerializeField] private StageManager stageManager;
    [SerializeField] private GameInput gameInput;

    [Header("Flags")]
    private bool isGameClear = false;

    [Header("UIs")]
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private GameObject gameClearUI;

    public float elapsedTime { get; private set; }
    public event Action StateChanged;
    public event Action OnGameOver;
    public event Action OnGameClear;

    private void Awake()
    {
        if (lifeManager == null)
            lifeManager = FindFirstObjectByType<LifeManager>();

        if (stageManager == null)
            stageManager = FindFirstObjectByType<StageManager>();

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

        if (stageManager != null)
            stageManager.OnAllWavesCompleted += HandleAllWavesCompleted;

        if (gameInput != null)
            gameInput.ToggleModeRequested += ToggleModePressed;

        // リトライボタンの押された処理をstaticで管理している
        // これで、リトライボタンが画面にいくつもあっても、
        // どれか押されたときにReloadSceneを走らせるという設計にできている。
        RetryButton.OnRetryRequested += ReloadScene;

    }

    private void OnDisable()
    {
        if (lifeManager != null)
            lifeManager.LifeZero -= GameOver;

        if (stageManager != null)
            stageManager.OnAllWavesCompleted -= HandleAllWavesCompleted;

        if (gameInput != null)
            gameInput.ToggleModeRequested -= ToggleModePressed;

        RetryButton.OnRetryRequested -= ReloadScene;
    }

    private void HandleAllWavesCompleted()
    {
        Debug.Log("handle");

        // Waveが終わった瞬間、ライフが0になった時を考慮する必要がある
        if (State != GameState.Playing)
            return;

        Debug.Log("handle playing");

        if (lifeManager.CurrentLife > 0)
            GameClear();
        else
        {
            Debug.Log("Handle側のGameover");
            // ライフ側のイベントと二重実行にならないように、
            // GameOver側でStateをチェックしている。ライフ側のイベントが先に走った場合、
            // Slowingになっているので、そもそもこの処理まで辿り着かないようにしている
            GameOver(); 
        }

    }

    private void SetUpGame()
    {
        // ゲーム開始時はEditモードから
        State = GameState.Edit;
        Time.timeScale = 0f;
        StateChanged?.Invoke();

        // 各種UIが表示されていたら、非公開にする
        if (gameOverUI != null)
            gameOverUI.SetActive(false);

        if (gameClearUI != null)
            gameClearUI.SetActive(false);

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
        if (State != GameState.Playing)
            return;

        OnGameOver?.Invoke();
        StartCoroutine(GameOverSequence());
    }

    // SlowMotionCoが終わるまで待機するためのメソッド
    // Coroutineを直列化して、Coroutineの完了を待てるようにする
    private IEnumerator GameOverSequence()
    {
        yield return StartCoroutine(SlowMotionCo());

        // スロー終了後に以下が実行される
        EndGame();
    }

    public void GameClear()
    {
        if (State != GameState.Playing)
            return;

        isGameClear = true;
        OnGameClear?.Invoke();
        StartCoroutine(GameClearSequence());
    }

    private IEnumerator GameClearSequence()
    {
        yield return StartCoroutine(SlowMotionCo());

        // スロー終了後に以下が実行される
        Debug.Log("GAME CLEAR!");
        EndGame();
    }

    // スロー演出を入れて、クリア / ゲームオーバーと別の関数に移る。
    private IEnumerator SlowMotionCo()
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
            if (gameClearUI != null)
            {
                gameClearUI.SetActive(true);
                if (gameClearUI.TryGetComponent<GameClearPanelController>(out var controller))
                    controller.PlayClearSequence();
            }
        }
        else
        {
            // ゲームオーバーUIの表示
            if (gameOverUI != null)
                gameOverUI.SetActive(true);

        }
    }

    // リトライ処理などで、現状シーンを再読込する
    private void ReloadScene()
    {
        var scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

}
