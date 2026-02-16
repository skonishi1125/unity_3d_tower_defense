using System;
using System.Collections;
using UnityEngine;

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
        Debug.Log("GAME OVER!");
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
        //AudioManager.Instance?.StopBgm();
        Time.timeScale = 0.5f;
        yield return new WaitForSecondsRealtime(3f);
        Time.timeScale = 1f; // 0にしちゃってもいいかも。1にすると、リザルト画面放置してるとどんどんゴールされて不具合につながりそう。
    }

    private void EndGame()
    {
        if (isGameClear)
        {
            // クリアの処理
        }
        else
        {
            // 失敗時の処理
        }
        // (共通で もういちど とか）
    }

}
