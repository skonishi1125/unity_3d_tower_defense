using UnityEngine;
public class RoadGenerator : MonoBehaviour
{
    // 床Objectの命名規則
    private const string SegmentPrefix = "[RoadSegment]";

    [Header("Source")]
    [SerializeField] private Transform[] points;

    // 床1枚分のプレート
    [Header("Prefab")]
    [SerializeField] private GameObject segmentPrefab;

    [Header("Shape")]
    [SerializeField] private float roadWidth = 1.0f;
    [SerializeField] private float roadThickness = 0.05f;
    [SerializeField] private float yOffset = 0.01f;

    [Header("Options")]
    [SerializeField] private bool flattenY = true;


    private void Start()
    {
        Generate();
    }

    [ContextMenu("Regenerate Road")]
    public void Generate()
    {
        // 道は2点間で作るので、長さが2未満なら何もしない
        if (points == null || points.Length < 2) return;
        if (segmentPrefab == null) return;

        Clear();

        for (int i = 0; i < points.Length - 1; i++)
        {
            // 例えば、point 0 と 1 が 1区間
            Transform a = points[i];
            Transform b = points[i + 1];
            if (a == null || b == null) continue;

            Vector3 p0 = a.position;
            Vector3 p1 = b.position;

            // 見下ろし型なので、y軸は固定する(Planeに貼り付ける)
            if (flattenY)
            {
                p0.y = 0f;
                p1.y = 0f;
            }

            // 向きと長さを取得
            Vector3 dir = p1 - p0;
            float length = dir.magnitude;
            if (length <= 0.0001f) continue;

            Vector3 mid = (p0 + p1) * 0.5f;
            mid.y += yOffset;

            // セグメント生成
            // 3Dオブジェクトは真ん中を基準にして、左右に伸びていくので、midを取得しておく必要がある
            GameObject seg = Instantiate(segmentPrefab, mid, Quaternion.identity, transform);
            seg.name = $"{SegmentPrefix} {i}";

            // 向き：CubeのZ(Forward)方向を p0->p1 に合わせる
            Vector3 flatDir = new Vector3(dir.x, 0f, dir.z);
            seg.transform.rotation = Quaternion.LookRotation(flatDir.normalized, Vector3.up);

            // 道Segment Objectの長さ調整
            // midに配置しているので、roadWidth = 1の場合はWaypointに配置したmidから0.5ずつ左右に伸びる
            // y軸は道の分厚さ
            // z軸は、p1 - p0の、絶対値の分だけ伸ばせば道に必要なの数値となる
            seg.transform.localScale = new Vector3(roadWidth, roadThickness, length);
        }
    }

    // 生成した道要素(Segment)をリセットする
    private void Clear()
    {
        // 子の中から自動生成分だけ消す（Prefab編集時の事故を防ぐためPrefixで限定）
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Transform child = transform.GetChild(i);
            if (child != null && child.name.StartsWith(SegmentPrefix))
            {
                Destroy(child.gameObject);
            }
        }
    }
}
