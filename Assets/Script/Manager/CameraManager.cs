using Unity.Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    // 特定のUIを開いている間は、マウスカーソルにカメラを追従させなくする
    [SerializeField] private Tutorial tutorial;
    [SerializeField] private PreStart prestart;

    // 編集画面時、マウスにカメラを追従させるための対象オブジェクト
    [SerializeField] private GameObject originalCameraTarget;

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

    [Header("Camera Shake")]
    [SerializeField] private CinemachineImpulseSource impulse;

    private void Awake()
    {
        if (prestart == null)
            Debug.LogWarning("CameraManagerにprestartが割り当てられていません。");

        if (tutorial == null)
            Debug.LogWarning("CameraManagerにTutorialパネルが割り当てられていません。");

        if (overviewVCam == null || buildVCam == null)
            Debug.LogWarning("CameraManagerに該当のカメラが割り当てられていません。");

        if (originalCameraTarget == null)
            Debug.LogWarning("CameraManagerにcameraTargetが割り当てられていません。");

        if (impulse == null)
            impulse = GetComponent<CinemachineImpulseSource>();

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

    private void Start()
    {
        // PreStartパネルから始まるので、カメラの追従は最初オフにしておく
        SetTrackingEnabled(false);
    }

    private void OnEnable()
    {
        if (stateManager != null)
        {
            stateManager.StateChanged += RefreshCameraPriority;
            stateManager.OnGameOver += DeathShake;
        }

        if (gameInput != null)
            gameInput.ZoomRequested += OnZoomRequested;

        if (tutorial != null)
            tutorial.OnPanelActive += OnTutorialActiveStateChanged;

        if (prestart != null)
            prestart.OnPreStartActive += OnTutorialActiveStateChanged;

    }

    private void OnDisable()
    {
        if (stateManager != null)
        {
            stateManager.StateChanged -= RefreshCameraPriority;
            stateManager.OnGameOver -= DeathShake;
        }

        if (gameInput != null)
            gameInput.ZoomRequested -= OnZoomRequested;

        if (tutorial != null)
            tutorial.OnPanelActive -= OnTutorialActiveStateChanged;

        if (prestart != null)
            prestart.OnPreStartActive -= OnTutorialActiveStateChanged;

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

    private void DeathShake()
    {
        Vector3 randomDirection = new Vector3(
            Random.Range(-1f, 1f),
            Random.Range(-1f, 1f),
            0f
        ).normalized;
        impulse.GenerateImpulse(randomDirection);
    }

    private void OnTutorialActiveStateChanged(bool isTutorialActive)
    {
        bool enableTracking = !isTutorialActive;
        SetTrackingEnabled(enableTracking);
    }

    // 現在のカメラターゲット情報を持たせておく
    // 例えばチュートリアルを開いた場合などは、この値をnullにして、
    // チュートリアル中にカメラ追従の動きをさせないようにする
    private void SetTrackingEnabled(bool isEnabled)
    {
        if (buildVCam != null)
        {
            if (isEnabled)
            {
                buildVCam.Follow = originalCameraTarget.transform;
                buildVCam.LookAt = originalCameraTarget.transform;
            }
            else
            {
                buildVCam.Follow = null;
                buildVCam.LookAt = null;
            }
        }
        else
        {
            Debug.LogWarning("CameraManager: buildVCam が未割当です。");
        }


    }

}
