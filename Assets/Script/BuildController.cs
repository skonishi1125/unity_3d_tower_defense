using UnityEngine;

public class BuildController : MonoBehaviour
{
    private Camera mainCamera;
    [SerializeField] private GridSystem grid;

    [Header("Place Setting")]
    private BuildMode currentBuildMode = BuildMode.Place;
    [SerializeField] private LayerMask groundLayerMask;
    [SerializeField] private GameObject towerPrefab;
    [SerializeField] private Transform cellHighlight;

    [Header("Ghost Setting")]
    private GameObject ghostInstance; // 設置予定物
    [SerializeField] private float highlightYOffset = 0.01f;
    [SerializeField] private float ghostYOffset = 0.0f;
    [SerializeField] private Material ghostMaterial;

    private void Awake()
    {
        mainCamera = Camera.main;
        if (grid == null)
            grid = FindFirstObjectByType<GridSystem>();
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

        Vector2Int cell = grid.WorldToCell(hit.point);

        // 建造モード, 配置不可セルにマウスがある場合はエフェクトを出さない。
        if (currentBuildMode == BuildMode.Place && grid.IsBlocked(cell))
        {
            if (ghostInstance != null)
                ghostInstance.SetActive(false);

            if (cellHighlight != null)
                cellHighlight.gameObject.SetActive(false);
            return;
        }

        // Ghost等の表示を中央から始めるために、中央の位置情報を取得
        Vector3 cellCenter = grid.CellToWorldCenter(cell);

        // セルハイライト処理
        if (cellHighlight != null)
        {
            cellHighlight.gameObject.SetActive(true);
            // 少しy軸を浮かせて、地面が光っているように演出
            cellHighlight.position = cellCenter + Vector3.up * highlightYOffset;
        }

        // Ghost表示処理
        // Ghostは建造モードのときだけ表示する。
        if (currentBuildMode == BuildMode.Place)
        {
            EnsureGhost();
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

    // 半透明なTower GameObjectを生成する。生成後は非activeとしておく
    private void EnsureGhost()
    {
        // すでに作られていたら走らせないでよい
        if (ghostInstance != null) return;
        if (towerPrefab == null) return;

        ghostInstance = Instantiate(towerPrefab);
        ghostInstance.name = "[Ghost] " + towerPrefab.name;
        var tower = ghostInstance.GetComponent<Tower>();
        if (tower != null)
            tower.SetState(TowerStateType.Ghost);

        // 衝突判定無効化
        foreach (var col in ghostInstance.GetComponentsInChildren<Collider>())
            col.enabled = false;

        // Visual配下だけ、マテリアルを差し替えたい
        // AttackRangeVisualのマテリアルは変えたくないので、
        // Visualを抜き出して、そこのRenderer周りのマテリアルだけを半透明にする
        var visual = ghostInstance.transform.Find("Visual");
        if (visual != null && ghostMaterial != null)
        {
            foreach (var r in visual.GetComponentsInChildren<Renderer>(true))
                r.material = ghostMaterial;
        }

        ghostInstance.SetActive(false);

    }

    // 塔の破壊処理
    private void DemolishTower()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundLayerMask))
        {
            Vector2Int cell = grid.WorldToCell(hit.point);
            GameObject tower;
            if (grid.TryRemoveTower(cell, out tower))
                Destroy(tower);
            else
                Debug.Log($"何も配置されていません: {cell}");

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
            Vector2Int cell = grid.WorldToCell(hit.point);
            if (grid.IsBlocked(cell))
            {
                Debug.Log($"その位置には配置できません。: {cell}");
                return;
            }

            Vector3 cellCenter = grid.CellToWorldCenter(cell);
            //DrawDebugLine(ray, hit, cellCenter, .2f);

            // GhostからTowerの実体を生成
            Quaternion rotate = Quaternion.identity;
            if (ghostInstance != null && ghostInstance.TryGetComponent<Tower>(out var ghostTower))
                rotate = ghostTower.TargetRotation;
            else if (ghostInstance != null)
                rotate = ghostInstance.transform.rotation;

            var tower = Instantiate(towerPrefab, cellCenter, rotate);
            var c = tower.GetComponent<Tower>();
            if (c != null)
                c.SetState(TowerStateType.Battle);
            if (!grid.TryAddTower(cell, tower))
            {
                Destroy(tower);
                Debug.Log($"登録に失敗しました: {cell}");
            }
        }
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
