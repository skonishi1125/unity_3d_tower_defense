using UnityEngine;

public enum GameState
{
    Edit,
    Playing,
    Result
}

public class StateManager : MonoBehaviour
{
    public static StateManager Instance;
    // ゲーム全体の状態
    public GameState State { get; private set; } = GameState.Edit;
    public float elapsedTime { get; private set; }

    [SerializeField] private LifeManager lifeManager;

    private void Awake()
    {
        if (lifeManager == null)
            lifeManager = FindFirstObjectByType<LifeManager>();
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
    }

    private void OnDisable()
    {
        if (lifeManager != null)
            lifeManager.LifeZero -= GameOver;
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
