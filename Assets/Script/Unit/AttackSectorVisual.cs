using UnityEngine;

// このcsを付与したObjectに、MeshFilter, MeshRendererを必須とするという宣言
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class AttackSectorVisual : MonoBehaviour
{
    [SerializeField] private int segments = 24;      // 円弧の分割数（見栄えと軽さのトレードオフ）
    [SerializeField] private float yOffset = 0.02f;  // 地面Z-fighting防止のため少し浮かす

    private UnitStatus status;
    private MeshFilter mf;
    private Mesh mesh;

    private void Awake()
    {
        // 攻撃範囲の詳細, 作る想定のMeshを用意
        status = GetComponentInParent<UnitStatus>();
        mf = GetComponent<MeshFilter>();
        mesh = new Mesh { name = "AttackSectorMesh" };
        mf.sharedMesh = mesh; // MeshRendererが描画するようにする

        Rebuild();
    }


    // 扇形のメッシュを、三角形を用いて擬似的に作る
    // https://scrapbox.io/skonishi1125-64754808/Unity_3dタワーディフェンス#69748c5d00000000009d4689
    public void Rebuild()
    {
        float range = status.GetAttackRange();
        float angle = status.GetViewingAngle();
        float half = angle * 0.5f; // forwardを正面として、±halfの扇形にする

        int vertCount = 1 + (segments + 1); // 三角形扇形 頂点の数。例えば三角形3つで表すなら5
        var vertices = new Vector3[vertCount]; // 頂点ベクトルを入れる配列 頂点分のインデックスがある
        var triangles = new int[segments * 3]; // verticesのどれ、どれ、どれをつなげて三角形にするかを格納する 頂点 x 3だけ用意

        vertices[0] = new Vector3(0f, yOffset, 0f); // 扇形の中心点 ローカル座標(0,yOffset,0)にしておく

        for (int i = 0; i <= segments; i++)
        {
            float t = (float)i / segments;// このループが今どのくらい進んでいるのかの割合
            float a = Mathf.Lerp(-half, half, t);// 例えば90°なら、tの値に応じて、-45° ~ +45°までの値を入れる
            Quaternion rot = Quaternion.Euler(0f, a, 0f); // aの分だけ回転するQuaternionを用意
            Vector3 dir = rot * Vector3.forward;// forwardを基準に、Quaternionの分だけ回転。
            vertices[1 + i] = dir * range + new Vector3(0f, yOffset, 0f); // i = 0は中心点として静的に値を入れているので、+1から入れていく
        }

        // 埋めたverticesの値について、どんな感じで三角形を構築していくのかを決定していく
        // triangles
        for (int i = 0; i < segments; i++)
        {
            // 1ループで、triangles[0][1][2]の3要素分決定していく
            int tri = i * 3;
            triangles[tri + 0] = 0; // vertices[0]をかならず使う（扇形の中心点）
            triangles[tri + 1] = 1 + i; // vertice[1+i]と
            triangles[tri + 2] = 1 + i + 1; // vertice[1+i+1]で、

            // 1ループ目なら、vertices[0][1][2]
            // 2ループ目なら、vertices[0][2][3] で三角形を作っていく感じ。
        }

        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals(); // 光の当たり方の再設定
        mesh.RecalculateBounds(); // カリング(描画対象判定）の調整
    }
}
