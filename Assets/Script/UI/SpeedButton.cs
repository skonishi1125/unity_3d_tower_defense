using TMPro;
using UnityEngine;

public class SpeedButton : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI speedButtonText;

    // 倍率と対応するテキストの定義
    private readonly float[] speedMultipliers = { 1.0f, 2.0f, 3.0f };
    private readonly string[] speedTexts = { ">", ">>", ">>>" };

    private int currentSpeedIndex = 0;

    private void Start()
    {
        UpdateTimeScaleAndUI();
    }

    // ボタンに配置する関数
    public void ToggleSpeed()
    {
        // インデックスを1つ進め、配列の長さを超えたら0に戻す
        currentSpeedIndex = (currentSpeedIndex + 1) % speedMultipliers.Length;
        UpdateTimeScaleAndUI();
    }

    private void UpdateTimeScaleAndUI()
    {
        Time.timeScale = speedMultipliers[currentSpeedIndex];

        // テキストの更新
        if (speedButtonText != null)
            speedButtonText.text = $"速度: {speedTexts[currentSpeedIndex]}" ;
    }

    private void OnDestroy()
    {
        // シーン遷移時や破棄時にタイムスケールが倍速のままになるのを防ぐ
        Time.timeScale = 1.0f;
    }

}
