using UnityEngine;

public enum UnitState
{
    None,
    Ghost = 1,
    Battle = 2,
}

public class Unit : MonoBehaviour
{
    public UnitState CurrentUnitState { get; private set; }

    public UnitStatus Status;

    // 建築時、確定した角度で建築する必要があるので参照する
    public Quaternion TargetRotation { get; private set; }
    private float rotationSpeed = 20f;
    [SerializeField] private GameObject attackRangeVisual;
    [SerializeField] private GameObject coolTimeUI;


    private void Awake()
    {
        TargetRotation = transform.rotation;

        if (attackRangeVisual == null)
        {
            var t = transform.Find("AttackRangeVisual");
            if (t != null)
                attackRangeVisual = t.gameObject;
        }

        Status = GetComponent<UnitStatus>();
    }

    private void Update()
    {
        // Unitの回転処理などはBuildController側で持つ
        if (CurrentUnitState == UnitState.Ghost)
            RotateSmoothly();
    }

    // 右に90°回転させよう
    public void Rotation()
    {
        // 絶対回転(world座標の、常に右を向けという処理になる)
        // transform.rotation = Quaternion.LookRotation(Vector3.right);

        // 相対回転: Rotate
        // transform.rotation = transform.Rotate(Vector3.right); // 間違い(代入はできない)
        // transform.Rotate(Vector3.right); // こっち
        // transform.Rotate(0f, 90f, 0f) // Eular角として、y軸90度回転を示す

        // 今回はSlerpを使いたいので、クォータニオンとして回転させる
        Quaternion delta = Quaternion.Euler(0f, 90f, 0f); // y軸90°
        TargetRotation = TargetRotation * delta;
    }

    private void RotateSmoothly()
    {
        // Angle: 2つの回転aとbの角度を返す。
        // Slerpで指定角度まで近づけたら、矯正てその値にする
        if (Quaternion.Angle(transform.rotation, TargetRotation) < 0.1f)
        {
            transform.rotation = TargetRotation;
            return;
        }

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            TargetRotation,
            Time.unscaledDeltaTime * rotationSpeed // 編集中はゲーム内時間が止まるので、unscaled
        );
    }


    public void SetState(UnitState type)
    {
        CurrentUnitState = type;

        switch (CurrentUnitState)
        {
            case UnitState.Ghost:
                if (attackRangeVisual != null)
                {
                    attackRangeVisual.SetActive(true);
                    coolTimeUI.SetActive(false);
                }

                break;
            case UnitState.Battle:
                if (attackRangeVisual != null)
                {
                    attackRangeVisual.SetActive(false);
                    coolTimeUI.SetActive(true);
                }
                break;
            default:
                break;
        }

    }

    public void SetTargetRotation(Quaternion rotation)
    {
        TargetRotation = rotation;
    }

}
