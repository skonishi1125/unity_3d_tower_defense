using UnityEngine;

public class UnitIcons : MonoBehaviour
{
    [SerializeField] private GameInput gameInput;
    [SerializeField] private UnitSelection selection;

    [SerializeField] private UnitCatalog catalog;
    [SerializeField] private GameObject iconPrefab;

    private void Awake()
    {
        if (gameInput == null)
            gameInput = FindFirstObjectByType<GameInput>();

        if (catalog == null)
            Debug.LogError("UnitIcons: catalog未割り当て");

        if (iconPrefab == null)
            Debug.LogError("UnitIcons: iconPrefab未割り当て");
    }

    private void Start()
    {
        CreateIconInChild();
    }

    private void OnEnable()
    {
        gameInput.SelectUnitRequested += OnSelectUnitRequested;
    }

    private void OnDisable()
    {
        gameInput.SelectUnitRequested -= OnSelectUnitRequested;
    }

    // 子要素にCatalogで登録された分だけ、Prefabを作る
    private void CreateIconInChild()
    {
        int key = 0;
        foreach (var unitDef in catalog.unitDefinitions)
        {
            // キーボード配列に沿って、10番目に生成されるキーは0とする
            key++;
            if (key == 10)
                key = 0;

                GameObject createIcon = Instantiate(iconPrefab, this.transform);
            createIcon.name = $"icon_{key}";
            UnitIcon unitIcon = createIcon.GetComponent<UnitIcon>();
            if (unitIcon != null)
                unitIcon.SetInfo(unitDef);


        }
    }

    private void OnSelectUnitRequested(int slot)
    {
        // カタログの中から、HotkeySlotが一致するものを探す
        foreach (var unitDef in catalog.unitDefinitions)
        {
            if (unitDef.HotkeySlot == slot)
            {
                selection.Select(unitDef);
                break;
            }
        }
    }
}
