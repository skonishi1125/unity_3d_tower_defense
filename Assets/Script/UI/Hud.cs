using TMPro;
using UnityEngine;

public class Hud : MonoBehaviour
{
    [SerializeField] private EconomyManager em;
    [SerializeField] private TextMeshProUGUI moneyAmount;

    private void Awake()
    {
        if (em == null)
            em = FindFirstObjectByType<EconomyManager>();

        // 初期表示（購読前にAwake通知が終わっている可能性があるため）
        UpdateMoneyAmount(em.CurrentMoney);
    }

    private void UpdateMoneyAmount(float currentMoney)
    {
        moneyAmount.text = $"{currentMoney} 円";
    }

    private void OnEnable()
    {
        if (em != null)
            em.MoneyChanged += UpdateMoneyAmount;

        // 保険で、有効化のたびに表示を正しくしておく
        if (em != null)
            UpdateMoneyAmount(em.CurrentMoney);
    }

    private void OnDisable()
    {
        if (em != null)
            em.MoneyChanged -= UpdateMoneyAmount;
    }


}
