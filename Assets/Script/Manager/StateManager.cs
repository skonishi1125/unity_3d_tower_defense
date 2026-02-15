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
    public float elapsedTime { get; private set; }

    [SerializeField] private LifeManager lifeManager;
    [SerializeField] private GameInput gameInput;

    private bool isGameClear = false;

    public event Action StateChanged;

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
        StartCoroutine(SlowMotionCo(false));
        Debug.Log("GAME OVER!");

        EndGame();
    }

    public void GameClear()
    {
        StartCoroutine(SlowMotionCo(true));
        Debug.Log("GAME CLEAR!");

        // クリア特有の処理をしたあと
        EndGame();
    }

    // スロー演出を入れて、Slowingから別の関数に移る。
    private IEnumerator SlowMotionCo(bool isGameClear)
    {
        State = GameState.Slowing;
        //AudioManager.Instance?.StopBgm();
        Time.timeScale = 0.5f;
        yield return new WaitForSecondsRealtime(3f);
        Time.timeScale = 1f;
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
