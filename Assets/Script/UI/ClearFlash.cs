using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ClearFlash : MonoBehaviour
{
    [Header("Flash Setting")]
    [SerializeField] private GameObject rootClearFlash;
    [SerializeField] private Image clearFlash;

    [Header("Tween")]
    [SerializeField] private float fadeIn = 0.03f;
    [SerializeField] private float hold = 0.02f;
    [SerializeField] private float fadeOut = 0.12f;
    [SerializeField] private float maxAlpha = 1.0f;
    [SerializeField] private bool useUnscaledTime = true;

    [Header("Flash SFX Clip")]
    [SerializeField] private AudioClip clearFlashSfx;

    private Tween _tween;

    private void Awake()
    {
        rootClearFlash.SetActive(false);
    }

    public void Play()
    {
        Debug.Log("clearflash!");

        if (clearFlash == null || rootClearFlash == null || clearFlashSfx == null)
            return;

        PlayClearSfx();

        // 多重再生対策
        _tween?.Kill();
        clearFlash.DOKill();

        rootClearFlash.SetActive(true);

        // 初期化（透明）
        var c = clearFlash.color;
        c.a = 0f;
        clearFlash.color = c;

        // 0 → maxAlpha → 0
        _tween = DOTween.Sequence()
            .Append(clearFlash.DOFade(maxAlpha, fadeIn))
            .AppendInterval(hold)
            .Append(clearFlash.DOFade(0f, fadeOut))
            .SetUpdate(useUnscaledTime)
            .OnComplete(() =>
            {
                rootClearFlash.SetActive(false);
            });
    }

    private void OnDisable()
    {
        _tween?.Kill();
    }

    private void PlayClearSfx()
    {
        // 一旦ここでBGMも止めているが、責務外かも。
        AudioManager.Instance?.StopBgm();
        AudioManager.Instance?.PlaySfx(clearFlashSfx);

    }

}
