using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class AttackSectorVisual : MonoBehaviour
{
    [SerializeField] private int segments = 24;      // 円弧の分割数（見栄えと軽さのトレードオフ）
    [SerializeField] private float yOffset = 0.02f;  // 地面Z-fighting防止のため少し浮かす

    private TowerStatus status;
    private MeshFilter mf;
    private Mesh mesh;

    private void Awake()
    {
        status = GetComponentInParent<TowerStatus>();
        mf = GetComponent<MeshFilter>();
        mesh = new Mesh { name = "AttackSectorMesh" };
        mf.sharedMesh = mesh;

        Rebuild();
    }

    // パラメータが変わる可能性があるなら、必要時に呼ぶ（例：強化/レベルアップ時など）
    public void Rebuild()
    {
        float range = status.GetAttackRange();          // Gizmoと同じ値を使う:contentReference[oaicite:3]{index=3}
        float angle = status.GetViewingAngle();
        float half = angle * 0.5f;

        // 頂点：中心 + 円弧上(segments+1点)
        int vertCount = 1 + (segments + 1);
        var vertices = new Vector3[vertCount];
        var triangles = new int[segments * 3];

        vertices[0] = new Vector3(0f, yOffset, 0f);

        for (int i = 0; i <= segments; i++)
        {
            float t = (float)i / segments;                 // 0..1
            float a = Mathf.Lerp(-half, half, t);          // -half..half（度）
            Quaternion rot = Quaternion.Euler(0f, a, 0f);
            Vector3 dir = rot * Vector3.forward;           // forward基準
            vertices[1 + i] = dir * range + new Vector3(0f, yOffset, 0f);
        }

        // 三角形：扇（中心0, i, i+1）
        for (int i = 0; i < segments; i++)
        {
            int tri = i * 3;
            triangles[tri + 0] = 0;
            triangles[tri + 1] = 1 + i;
            triangles[tri + 2] = 1 + i + 1;
        }

        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }
}
