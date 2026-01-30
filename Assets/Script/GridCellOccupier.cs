using UnityEngine;

// GameObjectに、同じコンポーネントをアタッチできないようにする
[DisallowMultipleComponent]
public class GridCellOccupier : MonoBehaviour
{
    [SerializeField] private GridSystem gridSystem;
    [SerializeField] private Collider targetCollider;

    private void Awake()
    {
        if (gridSystem == null)
            gridSystem = FindFirstObjectByType<GridSystem>();

        if (targetCollider == null)
            targetCollider = GetComponent<Collider>();
    }

    private void Start()
    {
        RegisterBlockedCells();
    }

    private void RegisterBlockedCells()
    {
        if (gridSystem == null || targetCollider == null)
            return;

        // Bounds: コライダーがWorld空間で締める範囲を箱で表したもの
        // ex) bのログ => Center: (-8.50, 0.00, 0.00), Extents: (1.50, 0.10, 2.00)
        // 中心 -8.5から、xに±1.5, yに±1, zに±2伸ばしたということ
        Bounds b = targetCollider.bounds;

        // boundsのx,z軸の最小 / 最大からセル範囲を得る
        Vector2Int minCell = gridSystem.WorldToCell(new Vector3(b.min.x, 0f, b.min.z));
        Vector2Int maxCell = gridSystem.WorldToCell(new Vector3(b.max.x, 0f, b.max.z));
        //Debug.Log(minCell);
        //Debug.Log(maxCell);
        //Debug.Log($"{b.min.x} {b.min.y} {b.min.z} {b.max.x} {b.max.y} {b.max.z}");

        // xが -10 から -7なら、そこから順に登録していく
        for (int x = minCell.x; x <= maxCell.x; x++)
        {
            // z軸(前方と後方)
            // minCellにとってはy軸のことになるので、そのあたりに気をつける
            for (int z = minCell.y; z <= maxCell.y; z++)
            {
                // 丸めた値で登録用のセルを作る
                var cell = new Vector2Int(x, z);
                if (IntersectsCellXZ(b, cell))
                    gridSystem.RegisterBlockedCell(cell);
            }
        }
    }


    // （厳密にはなくてもよいが、あると嬉しい）
    // 登録用のセルが、本当にboundsと触れているのか(Intersects:交差)チェックする
    private bool IntersectsCellXZ(Bounds b, Vector2Int cell)
    {
        Vector3 cellMin = gridSystem.CellToWorldMin(cell, 0f);
        // 例えばワールド座標で見たとき、(-10,-2)というセルは、
        // world: (-10.00, 0.00, -2.00) という座標に該当する

        // セル1つあたりのサイズ
        float s = gridSystem.CellSize;

        // cellMinは、セルの左下の最小点になる ※ □ の↙の点
        // なのでMaxを求めようと思ったら、セル1つあたりのサイズを足してやれば良い
        // (左下 から +1 すると、右下の座標が取れる = cellMax.xの座標）
        float cellMaxX = cellMin.x + s;
        float cellMaxZ = cellMin.z + s; // 左下から ↑ に +1して、maxの座標

        // bounds の max.x と cellMin の xを比較
        // Debug.Log($"{b.min.x} {b.max.x} {cellMin.x} {cellMaxX}");
        // 数直線の図にした
        // * https://scrapbox.io/skonishi1125-64754808/Unity_3dタワーディフェンス#697b0ce900000000003b66a5

        bool xOverlap = b.max.x >= cellMin.x && b.min.x <= cellMaxX;
        bool zOverlap = b.max.z >= cellMin.z && b.min.z <= cellMaxZ;
        //Debug.Log($"{xOverlap} {zOverlap}");

        return xOverlap && zOverlap;
    }

}
