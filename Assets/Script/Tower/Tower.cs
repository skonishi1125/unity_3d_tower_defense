using UnityEngine;

public enum TowerStateType
{
    None,
    Ghost = 1,
    Battle = 2,
}

public class Tower : MonoBehaviour
{
    private GameInput gameInput;

    public TowerStateType StateType;

    public TowerStatus Status;

    // 建築時、確定した角度で建築する必要があるので参照する
    public Quaternion TargetRotation;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private GameObject attackRangeVisual;
    [SerializeField] private GameObject coolTimeUI;


    private void Awake()
    {
        gameInput = FindFirstObjectByType<GameInput>();
        if (gameInput == null)
            Debug.LogWarning("Tower: gameInput未登録");

        TargetRotation = transform.rotation;

        if (attackRangeVisual == null)
        {
            var t = transform.Find("AttackRangeVisual");
            if (t != null)
                attackRangeVisual = t.gameObject;
        }

        Status = GetComponent<TowerStatus>();
    }

    // ゴースト状態のものは回転できる
    private void Update()
    {
        if (StateType == TowerStateType.Ghost)
            RotateSmoothly();
    }

    private void PressRotate()
    {
        Debug.Log("tower: InputSystemで: 右クリック");
        Rotation();
    }
    private void OnEnable()
    {
        gameInput.RotatePressed += PressRotate;
    }

    private void OnDisable()
    {
        gameInput.RotatePressed -= PressRotate;
    }


    // 右に90°回転させよう
    private void Rotation()
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
            Time.deltaTime * rotationSpeed
        );
    }


    public void SetState(TowerStateType type)
    {
        StateType = type;

        switch (StateType)
        {
            case TowerStateType.Ghost:
                if (attackRangeVisual != null)
                {
                    attackRangeVisual.SetActive(true);
                    coolTimeUI.SetActive(false);
                }

                break;
            case TowerStateType.Battle:
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

}
