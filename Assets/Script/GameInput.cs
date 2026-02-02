using System;
using UnityEngine;

public class GameInput : MonoBehaviour
{
    public GameInputSet Input { get; private set; }

    public event Action ToggleModeRequested; // ESC
    public event Action SelectBuildRequested; // E
    public event Action SelectDemolishRequested; // D
    public event Action ConfirmPressed; // 左クリック
    public event Action RotatePressed; // 右クリック

    private void Awake()
    {
        Input = new GameInputSet();

        // 戦闘
        Input.Global.ToggleMode.performed += _ => ToggleModeRequested?.Invoke();

        // 編集モード
        Input.Edit.SelectBuild.performed += _ => SelectBuildRequested?.Invoke();
        Input.Edit.SelectDemolish.performed += _ => SelectDemolishRequested?.Invoke();
        Input.Edit.Confirm.performed += _ => ConfirmPressed?.Invoke();
        Input.Edit.Rotate.performed += _ => RotatePressed?.Invoke();

    }

    private void OnEnable()
    {
        Input.Enable();
    }

    private void OnDisable()
    {
        Input.Disable();
    }

    // EditへInputSystemを切り替える
    public void SetModeEdit(bool enabled)
    {
        if (enabled)
            Input.Edit.Enable();
        else
            Input.Edit.Disable();
    }

}
