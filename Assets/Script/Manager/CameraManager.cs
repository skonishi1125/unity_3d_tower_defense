using Unity.Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    // カメラ優先度
    private const int PriorityActive = 20;
    private const int PriorityInactive = 10;

    [SerializeField] private StateManager stateManager;

    [SerializeField] private CinemachineCamera overviewVCam;
    [SerializeField] private CinemachineCamera buildVCam;

    private void Awake()
    {
        if (stateManager == null)
        {
            Debug.Log("CameraManager: stateManagerを自動割当します");
            stateManager = FindFirstObjectByType<StateManager>();
        }

        // 開始時のカメラ設定は、StateManager側でAwake時にInvokeされる
        // ただしAwake時、こちらのOnEnableの購読が間に合わない可能性があるので、
        // こちらでも実行しておく。
        RefreshCameraPriority();
    }

    private void OnEnable()
    {
        if (stateManager != null)
            stateManager.StateChanged += RefreshCameraPriority;
    }

    private void OnDisable()
    {
        if (stateManager != null)
            stateManager.StateChanged -= RefreshCameraPriority;
    }

    private void RefreshCameraPriority()
    {
        if (stateManager.State == GameState.Edit)
        {
            // Edit用カメラ
            overviewVCam.Priority = PriorityInactive;
            buildVCam.Priority = PriorityActive;
            Debug.Log("camerachange: buildVCam");
        }
        else // Playing, リザルト画面など
        {
            // 通常カメラに
            overviewVCam.Priority = PriorityActive;
            buildVCam.Priority = PriorityInactive;
            Debug.Log("camerachange: overviewVCam");
        }
    }

}
