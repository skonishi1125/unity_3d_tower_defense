using System;
using UnityEngine;

public class BuildController : MonoBehaviour
{
    private BuildMode currentBuildMode = BuildMode.None;
    [SerializeField] private LayerMask groundLayerMask;
    [SerializeField] private GameObject towerPrefab;
    [SerializeField] private Transform cellHighlight;


    private void Update()
    {
        SwitchMode();

        // ホバー更新（Place / Demolish のときだけ）
        if (currentBuildMode != BuildMode.None)
            UpdateHover();

        if (Input.GetMouseButtonDown(0))
        {
            if (currentBuildMode == BuildMode.Place)
                PlaceTower();
            else if (currentBuildMode == BuildMode.Demolish)
                DemolishTower();
        }


    }
    private void SwitchMode()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("Build Mode: Place");
            currentBuildMode = BuildMode.Place;
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            Debug.Log("Build Mode: Demolish");
            currentBuildMode = BuildMode.Demolish;
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("Build Mode: None");
            currentBuildMode = BuildMode.None;
        }
    }

    private void UpdateHover()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out var hit, Mathf.Infinity, groundLayerMask))
        {
            if (cellHighlight != null) cellHighlight.gameObject.SetActive(false);
            return;
        }

        Vector2Int cell = WorldToCell(hit.point);
        Vector3 center = CellToWorldCenter(cell, hit.point.y + 1f); // Z-fighting避け
        if (cellHighlight != null)
        {
            cellHighlight.gameObject.SetActive(true);
            cellHighlight.position = center;
        }
    }


    private void DemolishTower()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundLayerMask))
        {
            Vector3 p = hit.point;
            float s = .2f;
            DrawDebugLine(ray, hit, p, s);

            Collider[] colliders = Physics.OverlapSphere(hit.point, 0.5f);
            foreach (var collider in colliders)
            {
                if (collider.gameObject.layer == LayerMask.NameToLayer("Tower"))
                    Destroy(collider.gameObject);
            }
        }

    }

    private void PlaceTower()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundLayerMask))
        {
            //Vector3 p = hit.point;
            //float s = .2f;
            //DrawDebugLine(ray, hit, p, s);

            //Instantiate(towerPrefab, hit.point, Quaternion.identity);

            Vector2Int cell = WorldToCell(hit.point);
            Vector3 cellCenter = CellToWorldCenter(cell, hit.point.y);
            DrawDebugLine(ray, hit, cellCenter, .2f);
            Instantiate(towerPrefab, cellCenter, Quaternion.identity);

        }
    }

    private Vector2Int WorldToCell(Vector3 world)
    {
        // cellSize=1, origin=(0,0,0) 前提
        int x = Mathf.FloorToInt(world.x);
        int z = Mathf.FloorToInt(world.z);
        return new Vector2Int(x, z);
    }

    private Vector3 CellToWorldCenter(Vector2Int cell, float y)
    {
        // cellSize=1 なので +0.5 で中心
        return new Vector3(cell.x + 0.5f, y, cell.y + 0.5f);
    }


    // Cameraからマウスクリック位置に飛ばされるRayを赤色で可視化する
    private void DrawDebugLine(Ray ray, RaycastHit hit, Vector3 p, float s)
    {
        // 十字マーカー（赤）
        Debug.DrawLine(p - Vector3.right * s, p + Vector3.right * s, Color.red, 10f);
        Debug.DrawLine(p - Vector3.forward * s, p + Vector3.forward * s, Color.red, 10f);
        Debug.DrawLine(p - Vector3.up * s, p + Vector3.up * s, Color.red, 10f);

        // Ray 自体も可視化
        Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.red, 10f);
    }
}
