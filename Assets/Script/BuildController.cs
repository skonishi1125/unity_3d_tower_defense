using System.Collections.Generic;
using UnityEngine;

public class BuildController : MonoBehaviour
{
    private Camera mainCamera;
    [Header("Plane Setting")]
    // グリッド上に配置したTowerのデータ情報
    private readonly Dictionary<Vector2Int, GameObject> placedTowersDictionary = new();

    private BuildMode currentBuildMode = BuildMode.Place;
    [SerializeField] private LayerMask groundLayerMask;
    [SerializeField] private GameObject towerPrefab;
    [SerializeField] private Transform cellHighlight;


    [Header("Ghost Setting")]
    private GameObject ghostInstance; // 現状選択されている建造物が格納される想定
    [SerializeField] private float highlightYOffset = 0.01f;
    [SerializeField] private float ghostYOffset = 0.0f;
    [SerializeField] private Material ghostMaterial;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        SwitchMode();

        // ホバー更新（Place / Demolish のときだけ）
        if (currentBuildMode != BuildMode.None)
            DisplayPlaceableEffect();

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


    // 配置場所のグリッドに、四角くハイライト + Ghostを出す
    private void DisplayPlaceableEffect()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out var hit, Mathf.Infinity, groundLayerMask))
        {
            if (cellHighlight != null) cellHighlight.gameObject.SetActive(false);
            if (ghostInstance != null) ghostInstance.SetActive(false);
            return;
        }

        Vector2Int cell = WorldToCell(hit.point);
        bool canPlace = !placedTowersDictionary.ContainsKey(cell);

        // 配置済みの場所には何もエフェクトを出さない
        if (!canPlace && currentBuildMode == BuildMode.Place)
        {
            if (ghostInstance != null)
                ghostInstance.SetActive(false);

            cellHighlight.gameObject.SetActive(false);
            return;
        }

        Vector3 cellCenter = CellToWorldCenter(cell, hit.point.y);
        if (cellHighlight != null)
        {
            cellHighlight.gameObject.SetActive(true);
            cellHighlight.position = cellCenter + Vector3.up * highlightYOffset; // 少し浮かせて地面に埋もれないようにする
        }

        // ゴースト表示処理
        if (currentBuildMode == BuildMode.Place)
        {
            EnsureGhost();
            // 操作モードがPlaceで、現在のマウス位置に何も配置されてない場合はGhostを出す。
            if (ghostInstance != null)
            {
                ghostInstance.SetActive(true);
                ghostInstance.transform.position = cellCenter + Vector3.up * ghostYOffset;
            }
        }
        else
        {
            if (ghostInstance != null)
                ghostInstance.SetActive(false);
        }
    }

    // Place時、作成予定の半透明なTowerを画面に生成する
    private void EnsureGhost()
    {
        if (ghostInstance != null) return;
        if (towerPrefab == null) return;

        ghostInstance = Instantiate(towerPrefab);
        ghostInstance.name = "[Ghost] " + towerPrefab.name;

        // 衝突判定無効化
        foreach (var col in ghostInstance.GetComponentsInChildren<Collider>())
            col.enabled = false;

        if (ghostMaterial != null)
        {
            foreach (var r in ghostInstance.GetComponentsInChildren<Renderer>())
                r.material = ghostMaterial;
        }

        // 攻撃スクリプトなどあれば無効化しておく
        // foreach (var mb in ghostInstance.GetComponentsInChildren<MonoBehaviour>())
        //     mb.enabled = false;

        ghostInstance.SetActive(false);

    }


    private void DemolishTower()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundLayerMask))
        {
            Vector2Int cell = WorldToCell(hit.point);
            if (!placedTowersDictionary.TryGetValue(cell, out var tower))
            {
                Debug.Log($"何も配置されていません: {cell}");
                return;
            }
            Destroy(tower);
            placedTowersDictionary.Remove(cell);
        }

    }

    private void PlaceTower()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundLayerMask))
        {
            // グリッドを無視して生成する場合
            //Vector3 p = hit.point;
            //float s = .2f;
            //DrawDebugLine(ray, hit, p, s);
            //Instantiate(towerPrefab, hit.point, Quaternion.identity);

            // グリッドを考慮して生成する場合
            // 小数点をすべて取り除き、1グリッド中の中央の座標をcellCenterとして取得。
            // そこを基準にInstantiateしている
            Vector2Int cell = WorldToCell(hit.point);
            if (placedTowersDictionary.ContainsKey(cell))
            {
                Debug.Log($"その位置にはすでに配置されています: {cell}");
                return;
            }

            Vector3 cellCenter = CellToWorldCenter(cell, hit.point.y);
            DrawDebugLine(ray, hit, cellCenter, .2f);

            var tower = Instantiate(towerPrefab, cellCenter, Quaternion.identity);
            placedTowersDictionary.Add(cell, tower); // グリッドシステム二データとして保管

        }
    }

    private Vector2Int WorldToCell(Vector3 world)
    {
        // 1グリッド(cellSize)あたり 1, origin = (0,0,0) 前提
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
