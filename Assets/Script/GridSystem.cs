using System.Collections.Generic;
using UnityEngine;

public class GridSystem : MonoBehaviour
{
    private readonly Dictionary<Vector2Int, GameObject> towers = new();
    private readonly HashSet<Vector2Int> blocked = new(); // 指定のセルになにか含まれているかを高速で判定できる

    private float cellSize = 1f;
    private Vector3 origin = Vector3.zero; // グリッド左下の基準点

    public float CellSize => cellSize;
    public Vector3 Origin => origin;


    public bool IsBlocked(Vector2Int cell) => blocked.Contains(cell) || towers.ContainsKey(cell);

    public bool TryAddTower(Vector2Int cell, GameObject tower)
    {
        if (IsBlocked(cell)) return false;
        towers.Add(cell, tower);
        return true;
    }

    public bool TryRemoveTower(Vector2Int cell, out GameObject tower)
    {
        if (!towers.TryGetValue(cell, out tower)) return false;

        towers.Remove(cell);
        return true;
    }

    // ブロック用セル関連
    public void RegisterBlockedCell(Vector2Int cell) => blocked.Add(cell);
    public void UnregisterBlockedCell(Vector2Int cell) => blocked.Remove(cell);
    public void ClearBlockedCells() => blocked.Clear(); // TODO: 使う場合、既存の建物登録用スクリプトも再登録するよう修正

    // -------- 座標変換系 --------

    // 渡されたワールド座標を、セルとしての値に変換する
    public Vector2Int WorldToCell(Vector3 world)
    {
        // グリッドシステムの原点とワールド座標を比較した、グリッドシステムとしての座標(local)を取得
        // ex) 原点(0,0,0)  world(12.5,0,5) なら、(12.5,0,5)が返る
        // ex) 原点(12,0,5) world(12.5,0.5) なら、(0.5,0,0) が返る
        var local = world - origin;

        // FloorToInt: 小数点切り捨て
        // cellSizeを1に指定しているので、12.5/1 = 12マスとみなす
        int x = Mathf.FloorToInt(local.x / cellSize);
        int z = Mathf.FloorToInt(local.z / cellSize); // 5/1 = 5マスとみなす

        // ex) (12.5,0,5) なら、結果的に(12,5)として返す。
        return new Vector2Int(x, z);
    }

    // セル座標を、セルの中心ワールド座標に変換する
    public Vector3 CellToWorldCenter(Vector2Int cell, float y = 0f)
    {
        // ex) cell = (2,1) 原点origin = (0,0,0) の場合
        float wx = origin.x + (cell.x + 0.5f) * cellSize; // 0 + (2 + .5f) * 1 = 2.5
        float wz = origin.z + (cell.y + 0.5f) * cellSize; // 0 + (1 + .5f) * 1 = 1.5

        // (2.5, 0, 1.5)を返す
        // (2,1) の座標の中央のワールド座標値を返して、
        // そこでInstantiateすればそのマスすべてが埋まるようになる
        return new Vector3(wx, y, wz);
    }

    // セル座標を、ワールド座標に変換する
    public Vector3 CellToWorldMin(Vector2Int cell, float y = 0f)
    {
        float wx = origin.x + cell.x * cellSize;
        float wz = origin.z + cell.y * cellSize;
        return new Vector3(wx, y, wz);
    }

}
