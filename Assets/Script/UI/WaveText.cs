using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;

public class WaveText : MonoBehaviour
{
    [SerializeField] private StageManager stageManager;
    [SerializeField] private TextMeshProUGUI waveText;

    private Coroutine blinkRoutine;
    private Color defaultColor;
    private Vector3 defaultScale;

    private void Awake()
    {
        if (stageManager == null)
        {
            Debug.LogWarning("WaveText: stageManager未割り当てのため自動取得");
            stageManager = FindFirstObjectByType<StageManager>();
        }

        if (waveText != null)
        {
            defaultColor = waveText.color;
            defaultScale = waveText.transform.localScale;
        }

    }

    private void OnEnable()
    {
        if (stageManager != null)
            stageManager.WaveChanged += ChangeColor;
    }

    private void OnDisable()
    {
        if (stageManager != null)
            stageManager.WaveChanged -= ChangeColor;
    }

    private void ChangeColor()
    {
        if (blinkRoutine != null)
            return;

        blinkRoutine = StartCoroutine(ChangeCoroutine());

    }

    private IEnumerator ChangeCoroutine()
    {
        Sequence seq = DOTween.Sequence();
        seq.SetUpdate(true); // TimeScale影響をうけなくする

        // 色を黄色に、スケールを1.5倍にする
        seq.Append(waveText.DOColor(Color.yellow, 0.25f));
        seq.Join(waveText.transform.DOScale(defaultScale * 1.5f, 0.25f));

        // 色を元の色に、スケールを元のサイズに戻す
        seq.Append(waveText.DOColor(defaultColor, 0.25f));
        seq.Join(waveText.transform.DOScale(defaultScale, 0.25f));

        // Sequenceのアニメーション完了を待機
        yield return seq.WaitForCompletion();

        blinkRoutine = null;

    }

}
