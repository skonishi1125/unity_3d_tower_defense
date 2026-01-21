using System.Collections.Generic;
using UnityEngine;

public class GridSystem : MonoBehaviour
{
    private readonly Dictionary<Vector2Int, GameObject> towers = new();
    private readonly HashSet<Vector2Int> blocked = new(); // 指定のセルになにか含まれているかを高速で判定できる

    public bool IsBlocked(Vector2Int cell) => blocked.Contains(cell) || towers.ContainsKey(cell);

    public bool TryAddTower(Vector2Int cell, GameObject tower)
    {
        if (IsBlocked(cell))
            return false;
        towers.Add(cell, tower);
        return true;
    }

    public bool TryRemoveTower(Vector2Int cell, out GameObject tower)
    {
        if (!towers.TryGetValue(cell, out tower))
            return false;

        towers.Remove(cell);
        return true;
    }

    // ブロック用セル関連
    public void RegisterBlockedCell(Vector2Int cell) => blocked.Add(cell);
    public void UnregisterBlockedCell(Vector2Int cell) => blocked.Remove(cell);
    public void ClearBlockedCells() => blocked.Clear();

    // -------- 座標変換系 --------
    public Vector2Int WorldToCell(Vector3 world)
    {
        // 1グリッド(cellSize)あたり 1, origin = (0,0,0) 前提
        int x = Mathf.FloorToInt(world.x);
        int z = Mathf.FloorToInt(world.z);
        return new Vector2Int(x, z);
    }

    public Vector3 CellToWorldCenter(Vector2Int cell, float y)
    {
        // cellSize=1 なので +0.5 で中心
        return new Vector3(cell.x + 0.5f, y, cell.y + 0.5f);
    }


}
