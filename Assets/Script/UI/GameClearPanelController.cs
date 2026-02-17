using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum ClearRank
{
    S,
    A,
    B,
    C
}

[Serializable]
public struct RankCondition
{
    public ClearRank rank;
    public int minMoney; // 獲得するための最低金額
    public string rankMessage;
    public Color rankColor;
}

public class GameClearPanelController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private EconomyManager economyManager;

    [Header("UI Elements")]
    [SerializeField] private GameObject clearText;
    [SerializeField] private GameObject moneyPanel;
    [SerializeField] private GameObject rankPanel;
    [SerializeField] private GameObject rankTextPanel;
    [SerializeField] private GameObject buttonPanel;

    [Header("Timings")]
    [SerializeField] private float delayBeforeClearText = 0.5f;
    [SerializeField] private float delayBeforeButton = 1.0f;

    [Header("Money Data")]
    [SerializeField] private float delayBeforeMoney = 1.0f;
    [SerializeField] private float countUpDuration = 1.0f;
    [SerializeField] private TextMeshProUGUI moneyText;

    [Header("Rank Data")]
    [SerializeField] private float delayBeforeRank = 1.0f;
    [SerializeField] private float delayBeforeRankValue = 1.0f;
    [SerializeField] private float delayBeforeRankText = 1.0f;
    [SerializeField] private TextMeshProUGUI rankBaseText;
    [SerializeField] private TextMeshProUGUI rankValueText;
    [SerializeField] private TextMeshProUGUI rankMessageText;
    [SerializeField] private List<RankCondition> rankConditions;


    private void Awake()
    {
        if (economyManager == null)
            economyManager = FindFirstObjectByType<EconomyManager>();
    }

    public void PlayClearSequence()
    {
        // 初期状態としてすべて非表示にする
        clearText.SetActive(false);
        if (moneyPanel != null) moneyPanel.SetActive(false);
        if (rankPanel != null) rankPanel.SetActive(false);
        if (rankTextPanel != null) rankTextPanel.SetActive(false);
        if (buttonPanel != null) buttonPanel.SetActive(false);

        // コルーチンを開始
        StartCoroutine(ClearSequenceCo());
    }

    private IEnumerator ClearSequenceCo()
    {
        // 1. クリア表示
        yield return new WaitForSecondsRealtime(delayBeforeClearText);
        clearText.SetActive(true);

        // 2. 所持金欄の表示 (スロット演出は後で実装)
        yield return new WaitForSecondsRealtime(delayBeforeMoney);
        if (moneyPanel != null)
        {
            moneyPanel.SetActive(true);
            if (moneyText != null && economyManager != null)
            {
                // カウントアップ演出が完了するまでここで待機する
                yield return StartCoroutine(CountUpMoneyCo());
            }
        }

        // 3. ランク欄の表示 (ランク計算と一言は後で実装)
        yield return new WaitForSecondsRealtime(delayBeforeRank);
        if (rankPanel != null)
        {
            if (economyManager != null && rankValueText != null && rankMessageText != null)
            {
                RankCondition rank = EvaluateRank((int)economyManager.CurrentMoney);
                rankValueText.text = " "; // まだ空欄
                rankPanel.SetActive(true);
                yield return new WaitForSecondsRealtime(delayBeforeRankValue);
                rankValueText.text = rank.rank.ToString();
                rankValueText.transform.DOPunchScale(Vector3.one * 0.3f, 0.3f, 10, 1).SetUpdate(true);

                // SetActive対応自体はこのあと
                rankMessageText.text = rank.rankMessage;
            }
        }


        // 4. ランクについての一言
        yield return new WaitForSecondsRealtime(delayBeforeRankText);
        if (rankTextPanel != null) rankTextPanel.SetActive(true);

        // 5. ボタンパネル表示
        yield return new WaitForSecondsRealtime(delayBeforeButton);
        if (buttonPanel != null) buttonPanel.SetActive(true);

    }

    private IEnumerator CountUpMoneyCo()
    {
        int targetMoney = (int)economyManager.CurrentMoney; // 目標金額
        int currentDisplayMoney = 0; // だらららら、と表示する金額

        Tween countTween = DOTween.To(
            () => currentDisplayMoney,
            x =>
            {
                currentDisplayMoney = x;
                moneyText.text = $"¥ {currentDisplayMoney.ToString("N0")}";
            },
            targetMoney, // この値まで
            countUpDuration) // この時間かけて変化させる
        .SetUpdate(true);

        // DOTweenが完了するまで待機
        yield return countTween.WaitForCompletion();

        // 最終的な値の再代入と、スケールを少し大きくして元に戻す演出も加える
        moneyText.text = $"¥ {targetMoney.ToString("N0")}";

        // DOPunchScaleで一瞬テキストを拡大（1.3倍に設定、時間は0.3秒）
        moneyText.transform.DOPunchScale(Vector3.one * 0.3f, 0.3f, 10, 1).SetUpdate(true);
    }


    // 所持金をベースにしたRankConditionの値を返す。
    private RankCondition EvaluateRank(int finalMoney)
    {
        // rankConditionsはInspector上で minMoney が大きい順(S->A->B->C)に並んでいる前提
        foreach (var condition in rankConditions)
        {
            if (finalMoney >= condition.minMoney)
            {
                return condition;
            }
        }

        // リストが空だったり、全て満たせなかった場合のデフォルト（リストの一番下を返すなど）
        if (rankConditions.Count > 0)
            return rankConditions[rankConditions.Count - 1];

        return new RankCondition
        {
            rank = ClearRank.C,
            rankMessage = "C※リストが空でした。"
        };
    }



}
