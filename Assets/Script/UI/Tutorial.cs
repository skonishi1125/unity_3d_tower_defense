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
        gameInput.ActivePanelMode();

        wrapper.SetActive(true);
        OnPanelActive?.Invoke(true);
    }

    // x ボタンに付与する関数
    public void InactivePanel()
    {
        gameInput.InactivePanelMode();

        wrapper.SetActive(false);
        OnPanelActive?.Invoke(false);
    }


}
