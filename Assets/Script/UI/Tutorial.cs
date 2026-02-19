using System;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    [SerializeField] private GameInput gameInput;
    [SerializeField] private GameObject wrapper; // UI全体

    public event Action<bool> OnPanelActive;

    private void Awake()
    {
        if (gameInput == null)
            gameInput = FindFirstObjectByType<GameInput>();

        wrapper.SetActive(false);
    }

    private void OnEnable()
    {
        OpenTutorialButton.OnOpenTutorialRequested += ActivePanel;
    }

    private void OnDisable()
    {
        OpenTutorialButton.OnOpenTutorialRequested -= ActivePanel;
    }

    private void ActivePanel()
    {
        gameInput.Input.Global.Disable();
        gameInput.Input.Edit.Disable();
        gameInput.Input.Tutorial.Enable();
        Debug.Log("active");

        wrapper.SetActive(true);
        OnPanelActive?.Invoke(true);
    }

    // x ボタンに付与する関数
    public void InactivePanel()
    {
        gameInput.Input.Global.Enable();
        gameInput.Input.Edit.Enable();
        gameInput.Input.Tutorial.Disable();
        Debug.Log("inactive");

        wrapper.SetActive(false);
        OnPanelActive?.Invoke(false);
    }


}
