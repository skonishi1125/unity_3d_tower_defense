using System;
using UnityEngine;
public enum BuildMode
{
    None = 0,
    Build = 1,
    Demolish = 2,
}

public class BuildController : MonoBehaviour
{
    private Camera mainCamera;
    [SerializeField] private GridSystem grid;
    [SerializeField] private GameInput gameInput;
    [SerializeField] private StateManager stateManager;

    [Header("Place Setting")]
    public BuildMode CurrentBuildMode = BuildMode.Build;
    [SerializeField] private LayerMask groundLayerMask;
    [SerializeField] private GameObject towerPrefab;
    [SerializeField] private Transform cellHighlight;

    [Header("Ghost Setting")]
    private GameObject ghostInstance; // 設置予定物
    [SerializeField] private float highlightYOffset = 0.01f;
    [SerializeField] private float ghostYOffset = 0.0f;
    [SerializeField] private Material ghostMaterial;
    private float ghostCost;

    [Header("Dependencies")]
    // IEconomyを実装しているEconomyManagerを参照させる
    // SerializeFieldはMonoBehaviourでないと割り当てられないので、
    // IEconomyであってもSerializeFieldとして付与するように設計している
    [SerializeField] private MonoBehaviour economyProvider;
    private IEconomy economy;

    public event Action BuildModeChanged;

    private void Awake()
    {
        mainCamera = Camera.main;

        if (gameInput == null)
        {
            Debug.Log("BM: gameInputを自動割当します");
            gameInput = FindFirstObjectByType<GameInput>();
        }

        if (grid == null)
        {
            Debug.Log("BM: GridSystemを自動割当します");
            grid = FindFirstObjectByType<GridSystem>();
        }

        // as: キャストできるならそうする。無理ならnull。
        // MonoBehaviourとして受け取ったが、実際はIEconomyとして扱えるようにする
        economy = economyProvider as IEconomy;
        if (economy == null)
        {
            Debug.LogError("BM: economyProvider には IEconomyが必須です");
            enabled = false;
            return;
        }

        if (stateManager == null)
        {
            Debug.Log("BM: StateManagerがインスペクタ未割り当てのため自動割当します。");
            stateManager = FindFirstObjectByType<StateManager>();
        }
    }

    private void Update()
    {

        // ホバー更新（Place / Demolish のときだけ）
        if (CurrentBuildMode != BuildMode.None)
            DisplayPlaceableEffect();
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
        if (CurrentBuildMode == BuildMode.Build && grid.IsBlocked(cell))
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
        if (CurrentBuildMode == BuildMode.Build)
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
        {
            tower.SetState(TowerState.Ghost);
            ghostCost = tower.Status.GetCost();
        }

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

            // 建築可否及びコストの消費
            if (!economy.TrySpend(ghostCost))
                return;

            var tower = Instantiate(towerPrefab, cellCenter, rotate);
            var c = tower.GetComponent<Tower>();
            if (c != null)
                c.SetState(TowerState.Battle);
            if (!grid.TryAddTower(cell, tower))
            {
                Destroy(tower);
                economy.Refund(ghostCost);
                Debug.Log($"登録に失敗しました: {cell}");
            }
        }
    }

    // Key Config
    private void PressEdit()
    {
        Debug.Log("InputSystemで: Build Mode: Edit");
        CurrentBuildMode = BuildMode.Build;
        BuildModeChanged?.Invoke();
    }

    private void PressDemolish()
    {
        Debug.Log("InputSystemで: Build Mode: Demolish");
        CurrentBuildMode = BuildMode.Demolish;
        BuildModeChanged?.Invoke();
    }

    private void PressConfirm()
    {
        Debug.Log("InputSystemで: 左クリック");

        if (CurrentBuildMode == BuildMode.Build)
            PlaceTower();
        else if (CurrentBuildMode == BuildMode.Demolish)
            DemolishTower();
    }

    private void PressRotate()
    {
        Debug.Log("InputSystemで: 右クリック");

        // 3条件をここで集約
        if (!CanRotateGhost())
            return;

        if (ghostInstance.TryGetComponent<Tower>(out var ghostTower))
            ghostTower.Rotation();
    }

    private bool CanRotateGhost()
    {
        if (stateManager == null)
            return false;
        if (stateManager.State != GameState.Edit)
            return false;

        if (CurrentBuildMode != BuildMode.Build)
            return false;

        if (ghostInstance == null)
            return false;
        if (!ghostInstance.activeInHierarchy) // TODO: 詳細に調べる
            return false;

        if (!ghostInstance.TryGetComponent<Tower>(out var tower))
            return false;
        if (tower.CurrentTowerState != TowerState.Ghost)
            return false;

        // ゲーム全体が編集モードで
        // 編集中の状態が建築モードで
        // TowerがGhost状態のときは、回転できる
        return true;
    }

    private void OnEnable()
    {
        gameInput.SelectBuildRequested += PressEdit;
        gameInput.SelectDemolishRequested += PressDemolish;
        gameInput.ConfirmPressed += PressConfirm;
        gameInput.RotatePressed += PressRotate;
    }

    private void OnDisable()
    {
        gameInput.SelectBuildRequested -= PressEdit;
        gameInput.SelectDemolishRequested -= PressDemolish;
        gameInput.ConfirmPressed -= PressConfirm;
        gameInput.RotatePressed -= PressRotate;
    }


    // Debug
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
