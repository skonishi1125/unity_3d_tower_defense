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

    [Header("Damage Info")]
    [SerializeField] private TMP_Text damageText;
    [SerializeField] private TMP_Text intervelText;
    [SerializeField] private TMP_Text kbText;

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
        descriptionText.text = def.Description;

        if (def.UnitPrefab != null)
        {
            UnitStatus status = def.UnitPrefab.GetComponent<UnitStatus>();
            if (status != null)
            {
                costText.text = $"Costs: ¥ {status.GetCost()}";
                //damageText.text = $"ダメージ: {status.GetAttack()}";
                float interval = status.GetAttackInterval();
                if (interval > 0f)
                {
                    float attacksPerSec = 10f / interval;
                    float dps = status.GetAttack() / interval;
                    damageText.text = $"攻撃力: {status.GetAttack()} (DPS: {dps:F0})";
                    intervelText.text = $"頻度: 10 秒あたり {attacksPerSec:F1} 回";
                }
                else
                {
                    damageText.text = $"攻撃力: -";
                    intervelText.text = "攻撃速度: -";
                }
                kbText.text = $"ノックバック: {status.GetKnockbackRankText()}";
            }
            else
            {
                ClearStatusText();
            }
        }
        else
        {
            ClearStatusText();
        }

    }

    // 不具合でステータスが取得できなかった時に割り当てる関数
    private void ClearStatusText()
    {
        damageText.text = "-";
        intervelText.text = "-";
        kbText.text = "-";
        costText.text = $"Costs: ¥ - ";

    }

}
