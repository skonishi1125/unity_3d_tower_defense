using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitDetailPanel : MonoBehaviour
{
    [SerializeField] private UnitSelection selection;

    [Header("UI")]
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text costText;
    [SerializeField] private TMP_Text descriptionText;

    private void Awake()
    {
        if (selection == null)
            selection = FindFirstObjectByType<UnitSelection>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            Debug.Log("差し替えテスト");
            selection.DebugSelect();
        }
    }

    private void OnEnable()
    {
        selection.SelectedChanged += OnSelectedChanged;

        // 起動時に既に選択済みなら反映
        if (selection.Selected != null)
            OnSelectedChanged(selection.Selected);
    }

    private void OnDisable()
    {
        selection.SelectedChanged -= OnSelectedChanged;
    }

    private void OnSelectedChanged(UnitDefinition def)
    {
        iconImage.sprite = def.Icon;
        nameText.text = def.DisplayName;
        costText.text = $"Costs: ¥ {def.Cost}";
        descriptionText.text = def.Description;
    }

}
