using Unity.Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    // カメラ優先度
    private const int PriorityActive = 20;
    private const int PriorityInactive = 10;

    [SerializeField] private GameInput gameInput;
    [SerializeField] private StateManager stateManager;

    [Header("Overview")]
    [SerializeField] private CinemachineCamera overviewVCam;

    [Header("Build")]
    [SerializeField] private CinemachineCamera buildVCam;
    // ズームの感度と制限
    [SerializeField] private float zoomSpeed = 0.5f;
    [SerializeField] private float minDistance = 5f;
    [SerializeField] private float maxDistance = 30f;
    private CinemachinePositionComposer buildPositionComposer;

    private void Awake()
    {
        if (stateManager == null)
        {
            Debug.Log("CameraManager: stateManagerを自動割当します");
            stateManager = FindFirstObjectByType<StateManager>();
        }

        buildPositionComposer = buildVCam.GetComponent<CinemachinePositionComposer>();
        if (buildPositionComposer == null)
            Debug.LogWarning("CameraManager: buildPositionComposer未割り当て");

        // 開始時のカメラ設定は、StateManager側でAwake時にInvokeされる
        // ただしAwake時、こちらのOnEnableの購読が間に合わない可能性があるので、
        // こちらでも実行しておく。
        RefreshCameraPriority();
    }

    private void OnEnable()
    {
        if (stateManager != null)
            stateManager.StateChanged += RefreshCameraPriority;

        if (gameInput != null)
            gameInput.ZoomRequested += OnZoomRequested;

    }

    private void OnDisable()
    {
        if (stateManager != null)
            stateManager.StateChanged -= RefreshCameraPriority;

        if (gameInput != null)
            gameInput.ZoomRequested -= OnZoomRequested;

    }

    private void RefreshCameraPriority()
    {
        if (stateManager.State == GameState.Edit)
        {
            // Edit用カメラ
            overviewVCam.Priority = PriorityInactive;
            buildVCam.Priority = PriorityActive;
            //Debug.Log("camerachange: buildVCam");
        }
        else // Playing, リザルト画面など
        {
            // 通常カメラに
            overviewVCam.Priority = PriorityActive;
            buildVCam.Priority = PriorityInactive;
            //Debug.Log("camerachange: overviewVCam");
        }
    }

    // 現在の距離からスクロール量に応じて計算して最大値, 最小値でClampして返す
    private void OnZoomRequested(float scrollDelta)
    {
        // scrollDeltaが正（上回転）なら近づける（マイナス）、負なら遠ざける
        float currentDistance = buildPositionComposer.CameraDistance;
        float targetDistance = currentDistance - (scrollDelta * zoomSpeed);

        // 最大、最小範囲内に収める
        buildPositionComposer.CameraDistance = Mathf.Clamp(targetDistance, minDistance, maxDistance);
    }

}
