using DG.Tweening;
using TMPro;
using UnityEngine;

public class DamagePopUp : MonoBehaviour
{
    [SerializeField] private TextMeshPro textMesh;

    [Header("Text Movements")]
    [SerializeField] private float moveDistance = .5f;
    [SerializeField] private float duration = 1f;
    [SerializeField] private Ease moveEase = Ease.OutCirc;
    [SerializeField] private Ease fadeEase = Ease.InQuart;

    private void Awake()
    {
        if (textMesh == null)
            textMesh = GetComponent<TextMeshPro>();
    }

    // ダメージ表示生成後、この関数でセットアップする
    public void Setup(float damageAmount, Color? color = null)
    {
        textMesh.text = damageAmount.ToString("0"); // 小数点なし表示
        if (color.HasValue)
            textMesh.color = color.Value;

        // 上に移動
        transform.DOMoveY(transform.position.y + moveDistance, duration)
            .SetEase(moveEase);

        // フェードアウトしたらdestroyする
        textMesh.DOFade(0f, duration)
            .SetEase(fadeEase)
            .OnComplete(() =>
            {
                Destroy(gameObject);
            });
    }
}

