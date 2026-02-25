using Unity.Cinemachine;
using UnityEngine;
using DG.Tweening;

public class TitleCameraMoving : MonoBehaviour
{
    [Header("Overview")]
    [SerializeField] private CinemachineCamera overviewVCam;
    [SerializeField] private float rotatingLoopTime;

    private void Awake()
    {
        if (overviewVCam == null)
            Debug.LogWarning("TitleCameraMoving: 動かすカメラが未割当です");
    }

    private void Start()
    {
        StartOverviewCameraOrbit();
    }

    public void StartOverviewCameraOrbit()
    {
        // overviewVCamのTransformを直接回転させて、カメラを動かす
        // n秒かけてY軸を360度回転させ、それを無限ループ(Restart)する
        overviewVCam.transform.DORotate(new Vector3(0, 360, 0), rotatingLoopTime, RotateMode.FastBeyond360)
            .SetRelative(true)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Restart)
            .SetLink(gameObject); // DOTweenエラー対策
    }
}
