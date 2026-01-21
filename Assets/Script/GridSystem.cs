using System.Collections.Generic;
using UnityEngine;

public class GridSystem : MonoBehaviour
{
    // タワーが建てられているセル
    private readonly Dictionary<Vector2Int, GameObject> towers = new();

    private readonly HashSet<Vector2Int> blocked = new();


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

    public void RegisterBlocked(Vector2Int cell) => blocked.Add(cell);

    public void ClearBlocked() => blocked.Clear();




}
