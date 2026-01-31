using UnityEngine;

public class GameInput : MonoBehaviour
{
    public GameInputSet input { get; private set; }

    private void Awake()
    {
        input = new GameInputSet();
    }

    private void OnEnable()
    {
        input.Enable();
    }

    private void OnDisable()
    {
        input.Disable();
    }
}
