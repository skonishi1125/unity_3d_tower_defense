using System.Collections.Generic;
using UnityEngine;
public class RoadGenerator : MonoBehaviour
{
    // 床Objectの命名規則
    private const string SegmentPrefix = "[RoadSegment]";

    // グリッド周り
    [SerializeField] private GridSystem grid;
    private readonly List<Vector2Int> lastRoadCells = new();

    [Header("Source")]
    [SerializeField] private Transform[] points;

    // 床1枚分のプレート
    [Header("Prefab")]
    [SerializeField] private GameObject segmentPrefab;

    [Header("Shape")]
    [SerializeField] private float roadThickness = 0.05f;
    [SerializeField] private float yOffset = 0.01f; // Groundよりも少しだけ上に表示させるための補正値


    private void Start()
    {
        Generate();
    }

    public void Generate()
    {
        // 道は2点間で作るので、長さが2未満なら何もしない
        if (points == null || points.Length < 2) return;
        if (segmentPrefab == null) return;

        Clear(); // 見た目Objectの削除
        ClearRoadCellsOnGrid(); // グリッドシステムのデータの削除

        for (int i = 0; i < points.Length - 1; i++)
        {
            // 例えば、point 0 と 1 が 1区間
            Transform a = points[i];
            Transform b = points[i + 1];
            if (a == null || b == null) continue;

            RegisterBlockAndPlaceTiles(a.position, b.position);
        }
    }

    // 辿っていったセルを登録していく
    // Bresenham(格子線分アルゴリズム)で、p0 -> p1を結ぶ線を1セルずつ辿る
    private void RegisterBlockAndPlaceTiles(Vector3 p0, Vector3 p1)
    {
        var c0 = grid.WorldToCell(p0);
        var c1 = grid.WorldToCell(p1);

        foreach (var cell in EnumerateCells(c0, c1))
        {
            // データとして登録
            grid.RegisterBlockedCell(cell);
            lastRoadCells.Add(cell);

            // セルの中央の値(1*1なら、0.5, 0.5)を取って、そこを基準にタイルを置く
            Vector3 pos = grid.CellToWorldCenter(cell);
            pos.y += yOffset;

            GameObject tile = Instantiate(segmentPrefab, pos, Quaternion.identity, gameObject.transform);
            tile.name = $"{SegmentPrefix} {cell.x},{cell.y}";

            // 1セルタイル前提なら scale は固定
            tile.transform.localScale = new Vector3(1f, roadThickness, 1f);
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

    // 生成した道要素について、データとしても削除する
    private void ClearRoadCellsOnGrid()
    {
        if (grid == null) return;

        foreach (var cell in lastRoadCells)
            grid.UnregisterBlockedCell(cell);

        lastRoadCells.Clear();
    }

    // Bresenhamのアルゴリズム
    // (1,1) -> (6,3)とかの例なら、横に5, 縦に2 というようなルート結果をうまく出してくれる。
    private IEnumerable<Vector2Int> EnumerateCells(Vector2Int a, Vector2Int b)
    {
        int x0 = a.x;
        int y0 = a.y;
        int x1 = b.x;
        int y1 = b.y;

        int dx = Mathf.Abs(x1 - x0); // 目的地まで、横に何マスか
        int dy = Mathf.Abs(y1 - y0); // 縦に何マスか
        // +方向に進むか-方向に進むか
        int sx = x0 < x1 ? 1 : -1;
        int sy = y0 < y1 ? 1 : -1;
        // 横 or 縦に寄っていないかの指標
        int err = dx - dy;

        while (true)
        {
            yield return new Vector2Int(x0, y0);
            if (x0 == x1 && y0 == y1) yield break;

            int e2 = 2 * err;
            if (e2 > -dy) { err -= dy; x0 += sx; }
            if (e2 < dx) { err += dx; y0 += sy; }
        }
    }


}
