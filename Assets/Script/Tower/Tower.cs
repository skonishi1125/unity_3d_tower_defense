using System;
using UnityEngine;

public enum TowerStateType
{
    None,
    Ghost = 1,
    Battle = 2,
}

public class Tower : MonoBehaviour
{
    public TowerStateType StateType;
    private Quaternion targetRotation;
    [SerializeField] private float rotationSpeed = 5f;

    private void Awake()
    {
        targetRotation = transform.rotation;
    }

    // ゴースト状態のものは回転できる
    private void Update()
    {
        if (Input.GetMouseButtonDown(1) && StateType == TowerStateType.Ghost)
            Rotation();

        // 回転ができたら、targetRotationの値に応じてSlerpで振り向く
        RotateSmoothly();
    }



    // 右に90°回転させよう
    private void Rotation()
    {
        // 絶対回転(world座標の、常に右を向けという処理になる)
         //transform.rotation = Quaternion.LookRotation(Vector3.right);

        // 相対回転: Rotate
        // transform.rotation = transform.Rotate(Vector3.right); // 間違い(代入はできない)
        // transform.Rotate(Vector3.right); // こっち
        // transform.Rotate(0f, 90f, 0f) // Eular角として、y軸90度回転を示す

        // 今回はSlerpを使いたいので、クォータニオンとして回転させる
        Quaternion delta = Quaternion.Euler(0f, 90f, 0f); // y軸90°
        targetRotation = targetRotation * delta;


    }

    private void RotateSmoothly()
    {
        if (StateType != TowerStateType.Ghost)
            return;

        // Angle: 2つの回転aとbの角度を返す。
        // Slerpで指定角度まで近づけたら、矯正てその値にする
        if (Quaternion.Angle(transform.rotation, targetRotation) < 0.1f)
        {
            transform.rotation = targetRotation;
            return;
        }

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            Time.deltaTime * rotationSpeed
        );
    }


    public void SetState(TowerStateType type)
    {
        StateType = type;
    }

}
