using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour
{
    public GameInputSet Input { get; private set; }

    public Vector2 PointerPosition { get; private set; }
    public float ScrollDelta { get; private set; }

    public event Action ToggleModeRequested; // Global: ESC
    public event Action SelectBuildRequested; // E
    public event Action SelectDemolishRequested; // D
    public event Action ConfirmPressed; // 左クリック
    public event Action RotatePressed; // 右クリック
    public event Action<int> SelectUnitRequested;
    public event Action<float> ZoomRequested; // マウスホイール

    private void Awake()
    {
        Input = new GameInputSet();
    }

    private void OnEnable()
    {
        // Globalは常に有効にする
        Input.Global.Enable();
        // 最初は編集モードから始まるので、編集のマッピングを有効にする
        Input.Edit.Enable();

        // ゲーム全体
        Input.Global.ToggleMode.performed += _ => ToggleModeRequested?.Invoke();
        Input.Global.Point.performed += OnPoint;
        Input.Global.Point.canceled += OnPoint;

        // 編集モード
        Input.Edit.SelectBuild.performed += _ => SelectBuildRequested?.Invoke();
        Input.Edit.SelectDemolish.performed += _ => SelectDemolishRequested?.Invoke();
        Input.Edit.Confirm.performed += _ => ConfirmPressed?.Invoke();
        Input.Edit.Rotate.performed += _ => RotatePressed?.Invoke();
        Input.Edit.SelectUnit.performed += OnSelectUnitPerformed;
        Input.Edit.Zoom.performed += OnZoom;
        Input.Edit.Zoom.canceled += OnZoom;

        // チュートリアル画面
        Input.Tutorial.Confirm.performed += _ => ConfirmPressed?.Invoke();
    }

    private void OnDisable()
    {
        // ゲーム全体
        Input.Global.ToggleMode.performed -= _ => ToggleModeRequested?.Invoke();
        // ↓だと、購読解除できない
        // Input.Global.Point.performed -= ctx => PointerPosition = ctx.ReadValue<Vector2>();
        Input.Global.Point.performed -= OnPoint;
        Input.Global.Point.canceled -= OnPoint;

        // 編集モード
        Input.Edit.SelectBuild.performed -= _ => SelectBuildRequested?.Invoke();
        Input.Edit.SelectDemolish.performed -= _ => SelectDemolishRequested?.Invoke();
        Input.Edit.Confirm.performed -= _ => ConfirmPressed?.Invoke();
        Input.Edit.Rotate.performed -= _ => RotatePressed?.Invoke();
        Input.Edit.SelectUnit.performed -= OnSelectUnitPerformed;
        Input.Edit.Zoom.performed -= OnZoom;
        Input.Edit.Zoom.canceled -= OnZoom;

        // チュートリアル画面
        Input.Tutorial.Confirm.performed -= _ => ConfirmPressed?.Invoke();


        Input.Edit.Disable();
        Input.Global.Disable();

        Input.Disable(); // これだけでいいかも？

    }

    private void OnPoint(InputAction.CallbackContext ctx)
    {
        PointerPosition = ctx.ReadValue<Vector2>();
    }

    // Globalは常に有効にしたまま、EditのInputSystemを有効 / 無効とする
    public void SetModeEdit(bool enabled)
    {
        if (enabled)
            Input.Edit.Enable();
        else
            Input.Edit.Disable();
    }

    // SelectUnitで登録された数字キーの数字を返しつつ、イベントを実行
    private void OnSelectUnitPerformed(InputAction.CallbackContext ctx)
    {
        // ctx.control.name には押されたキーの名称（"1"や"2"）が入る
        if (int.TryParse(ctx.control.name, out int slot))
            SelectUnitRequested?.Invoke(slot);
    }

    private void OnZoom(InputAction.CallbackContext ctx)
    {
        Vector2 scrollValue = ctx.ReadValue<Vector2>();
        ScrollDelta = scrollValue.y;

        if (ScrollDelta != 0)
            ZoomRequested?.Invoke(ScrollDelta);

        //Debug.Log(ScrollDelta);
    }

    // 最初のPreStart, Tutorialが開いているときのinput処理
    public void ActivePanelMode()
    {
        Input.Global.Disable();
        Input.Edit.Disable();
        Input.Tutorial.Enable();
    }

    public void InactivePanelMode()
    {
        Input.Global.Enable();
        Input.Edit.Enable();
        Input.Tutorial.Disable();
    }

}
