using UnityEngine;

public class CameraPointerFollow : MonoBehaviour
{
    [SerializeField] private GameInput gameInput;
    [SerializeField] private float moveRange = 5f;

    private void Awake()
    {
        if (gameInput == null)
        {
            Debug.Log("CameraPointerFollow: gameInputを自動割当します");
            gameInput = FindFirstObjectByType<GameInput>();
        }
    }

    private void Update()
    {
        // マウスの画面座標（0〜1）を取得
        Vector2 screenPos = new Vector2(
            (gameInput.PointerPosition.x / Screen.width) * 2 - 1,
            (gameInput.PointerPosition.y / Screen.height) * 2 - 1
        );

        // 中心を (0, 0) に変換 (-0.5 〜 0.5)
        Vector2 offset = screenPos - new Vector2(0.5f, 0.5f);

        transform.localPosition = new Vector3(screenPos.x * moveRange, 0, screenPos.y * moveRange);
    }


}
