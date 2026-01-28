using UnityEngine;

public class BillBoard : MonoBehaviour
{
    [SerializeField] Camera targetCamera;

    private void Awake()
    {
        if (!targetCamera)
            targetCamera = Camera.main;
    }

    private void LateUpdate()
    {
        if (!targetCamera)
            return;

        // カメラと同じ向きに合わせて、体力バーをこちらに常に向ける
        transform.rotation = targetCamera.transform.rotation;
    }


}
