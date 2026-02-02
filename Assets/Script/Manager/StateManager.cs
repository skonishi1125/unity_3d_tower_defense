using System;
using UnityEngine;

public enum GameState
{
    Edit,
    Playing,
    Result
}

public class StateManager : MonoBehaviour
{
    public GameState State { get; private set; }
    public float elapsedTime { get; private set; }

    [SerializeField] private LifeManager lifeManager;
    [SerializeField] private GameInput gameInput;

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
        Debug.Log("GAME OVER!");
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
