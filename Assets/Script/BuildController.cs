using System;
using UnityEngine;
using UnityEngine.EventSystems;
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
    [SerializeField] private UnitSelection unitSelection;

    [Header("Place Setting")]
    public BuildMode CurrentBuildMode = BuildMode.Build;
    [SerializeField] private LayerMask groundLayerMask;
    [SerializeField] private Transform cellHighlight;
    [SerializeField] private Transform demolishCellHighlight;

    [Header("Ghost Setting")]
    private GameObject ghostInstance; // 設置予定物
    [SerializeField] private float highlightYOffset = 0.01f;
    [SerializeField] private float ghostYOffset = 0.0f;
    [SerializeField] private Material ghostMaterial;

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
            Debug.Log("BC: gameInputを自動割当します");
            gameInput = FindFirstObjectByType<GameInput>();
        }

        if (grid == null)
        {
            Debug.Log("BC: GridSystemを自動割当します");
            grid = FindFirstObjectByType<GridSystem>();
        }

        if (unitSelection == null)
        {
            Debug.Log("BC: unitSelectionを自動割当します");
            unitSelection = FindFirstObjectByType<UnitSelection>();
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

        if (cellHighlight != null) cellHighlight.gameObject.SetActive(false);
        if (demolishCellHighlight != null) demolishCellHighlight.gameObject.SetActive(false);

    }

    private void Update()
    {
        // ホバー更新（Place / Demolish のときだけ）
        if (CurrentBuildMode != BuildMode.None)
            DisplayPlaceableEffect();
    }

    // Ghostの出現処理
    // 配置場所のグリッドに、四角くハイライト + Ghostを出す
    private void DisplayPlaceableEffect()
    {
        // マウスポインタがUIにある場合は、Ghostを出さない
        // EventSystem.current.IsPointerOverGameObject() でマウスがCanvas上にあるかどうか判定できる
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            HideHighlights();
            return;
        }

        // Playing（戦闘中）は、Ghostを出さない
        // ※画面にGhostがある状態から戦闘に遷移したときは、ESCが押されたときに別途処理でActiveをfalseにしている
        if (stateManager.State == GameState.Playing)
            return;

        // 地面判定でない部分には、Ghostを出さない
        Ray ray = mainCamera.ScreenPointToRay(gameInput.PointerPosition);
        if (!Physics.Raycast(ray, out var hit, Mathf.Infinity, groundLayerMask))
        {
            HideHighlights();
            return;
        }

        Vector2Int cell = grid.WorldToCell(hit.point);

        // 建造モード, 配置不可セルにマウスがある場合はGhostを出さない
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

        // Ghost, セル表示処理
        if (CurrentBuildMode == BuildMode.Build)
        {
            if (cellHighlight != null)
            {
                if (!cellHighlight.gameObject.activeSelf)
                    cellHighlight.gameObject.SetActive(true);
                // 少しy軸を浮かせて、地面が光っているように演出
                cellHighlight.position = cellCenter + Vector3.up * highlightYOffset;
            }

            EnsureGhost(); // Ghostは建造モードのときだけ表示する
            if (ghostInstance != null)
            {
                if (!ghostInstance.activeSelf)
                    ghostInstance.SetActive(true);
                ghostInstance.transform.position = cellCenter + Vector3.up * ghostYOffset;
            }
        }
        else if (CurrentBuildMode == BuildMode.Demolish)
        {
            if (demolishCellHighlight != null)
            {
                if (!demolishCellHighlight.gameObject.activeSelf)
                    demolishCellHighlight.gameObject.SetActive(true);
                demolishCellHighlight.position = cellCenter + Vector3.up * highlightYOffset;
            }

            if (ghostInstance != null)
                ghostInstance.SetActive(false);
        }
    }

    private void HideHighlights()
    {
        if (cellHighlight != null) cellHighlight.gameObject.SetActive(false);
        if (demolishCellHighlight != null) demolishCellHighlight.gameObject.SetActive(false);
        if (ghostInstance != null) ghostInstance.SetActive(false);
    }

    // 半透明なTower GameObjectを生成する。生成後は非activeとしておく
    private void EnsureGhost()
    {
        // unitSelectionが取得できていなければ中断
        if (unitSelection.Selected == null || unitSelection.Selected.UnitPrefab == null) return;

        // すでに作られていたら走らせないでよい
        if (ghostInstance != null) return;

        ghostInstance = Instantiate(unitSelection.Selected.UnitPrefab);
        ghostInstance.name = "[Ghost] " + unitSelection.Selected.UnitPrefab.name;
        var tower = ghostInstance.GetComponent<Tower>();
        if (tower != null)
            tower.SetState(TowerState.Ghost);

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
        Ray ray = mainCamera.ScreenPointToRay(gameInput.PointerPosition);
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
        Ray ray = mainCamera.ScreenPointToRay(gameInput.PointerPosition);
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

            UnitDefinition selectedUnit = unitSelection.Selected;
            if (selectedUnit == null) return;

            // 建築可否及びコストの消費
            if (!economy.TrySpend(selectedUnit.Cost))
                return;

            // 現在選択中のユニットのPrefabを使う
            if (unitSelection.Selected == null) return;

            var targetPrefab = unitSelection.Selected.UnitPrefab;

            var towerObject = Instantiate(targetPrefab, cellCenter, rotate);
            var c = towerObject.GetComponent<Tower>();
            if (c != null)
                c.SetState(TowerState.Battle);
            if (!grid.TryAddTower(cell, towerObject))
            {
                Destroy(towerObject);
                economy.Refund(selectedUnit.Cost);
                Debug.Log($"登録に失敗しました: {cell}");
            }
        }
    }

    // Key Config
    private void PressToggleMode()
    {
        // BuildController側での責務は、ghostとセルハイライトを切ることだけ
        // GameStateを切り替える処理は、StateManager側が受け持っている
        if (cellHighlight != null) cellHighlight.gameObject.SetActive(false);
        if (demolishCellHighlight != null) demolishCellHighlight.gameObject.SetActive(false);
        if (ghostInstance != null) ghostInstance.SetActive(false);
    }
    private void PressEdit()
    {
        CurrentBuildMode = BuildMode.Build;
        ApplyModeVisuals();
        BuildModeChanged?.Invoke();
    }

    private void PressDemolish()
    {
        CurrentBuildMode = BuildMode.Demolish;
        ApplyModeVisuals();
        BuildModeChanged?.Invoke();
    }

    private void PressConfirm()
    {
        // UIクリック時は、建築や解体の実行をしない
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        if (CurrentBuildMode == BuildMode.Build)
            PlaceTower();
        else if (CurrentBuildMode == BuildMode.Demolish)
            DemolishTower();
    }

    private void PressRotate()
    {
        if (!CanRotateGhost())
        {
            Debug.Log("回転できません");
            return;
        }

        if (ghostInstance.TryGetComponent<Tower>(out var ghostTower))
            ghostTower.Rotation();
    }

    // Ghostと、どちらのCellHighlightをactiveにするか決定する
    private void ApplyModeVisuals()
    {
        if (cellHighlight != null)
            cellHighlight.gameObject.SetActive(false);
        if (demolishCellHighlight != null)
            demolishCellHighlight.gameObject.SetActive(false);

        if (CurrentBuildMode == BuildMode.Build)
        {
            if (cellHighlight != null)
                cellHighlight.gameObject.SetActive(true);
        }
        else if (CurrentBuildMode == BuildMode.Demolish)
        {
            if (demolishCellHighlight != null)
                demolishCellHighlight.gameObject.SetActive(true);
        }

        if (ghostInstance != null)
            ghostInstance.SetActive(CurrentBuildMode == BuildMode.Build);
    }

    private bool CanRotateGhost()
    {
        if (stateManager == null)
            return false;

        //Debug.Log("1");


        if (stateManager.State != GameState.Edit)
            return false;

        //Debug.Log("2");


        if (CurrentBuildMode != BuildMode.Build)
            return false;

        //Debug.Log("3");


        if (ghostInstance == null)
            return false;

        //Debug.Log("4");


        if (!ghostInstance.activeInHierarchy) // TODO: 詳細に調べる
            return false;

        //Debug.Log("5");

        if (!ghostInstance.TryGetComponent<Tower>(out var tower))
            return false;

        //Debug.Log("6");

        if (tower.CurrentTowerState != TowerState.Ghost)
            return false;

        //Debug.Log("7");


        // ゲーム全体が編集モードで
        // 編集中の状態が建築モードで
        // TowerがGhost状態のときは、回転できる
        return true;
    }

    // 選択中のユニットが変化したときの差し替え処理
    private void OnUnitSelectionChanged(UnitDefinition newUnit)
    {
        // 1. 古いGhostがあれば削除する
        if (ghostInstance != null)
        {
            Destroy(ghostInstance);
            ghostInstance = null;
        }

        // 2. 新しいユニットがnull（未選択）ならここで終了
        if (newUnit == null) return;

        if (CurrentBuildMode == BuildMode.Build && newUnit != null)
            EnsureGhost();
    }

    private void OnEnable()
    {
        gameInput.ToggleModeRequested += PressToggleMode;
        gameInput.SelectBuildRequested += PressEdit;
        gameInput.SelectDemolishRequested += PressDemolish;
        gameInput.ConfirmPressed += PressConfirm;
        gameInput.RotatePressed += PressRotate;

        if (unitSelection != null)
            unitSelection.SelectedChanged += OnUnitSelectionChanged;

    }

    private void OnDisable()
    {
        gameInput.ToggleModeRequested -= PressToggleMode;
        gameInput.SelectBuildRequested -= PressEdit;
        gameInput.SelectDemolishRequested -= PressDemolish;
        gameInput.ConfirmPressed -= PressConfirm;
        gameInput.RotatePressed -= PressRotate;

        if (unitSelection != null)
            unitSelection.SelectedChanged -= OnUnitSelectionChanged;
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
