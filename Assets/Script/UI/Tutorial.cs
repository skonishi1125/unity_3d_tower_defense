using System;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    [SerializeField] private GameInput gameInput;
    [SerializeField] private GameObject wrapper; // UI全体

    [Header("Pages")]
    [SerializeField] private GameObject[] pages;
    private int currentPageIndex;

    public event Action<bool> OnPanelActive;

    private void Awake()
    {
        if (gameInput == null)
            gameInput = FindFirstObjectByType<GameInput>();

        wrapper.SetActive(false);
    }

    private void OnEnable()
    {
        OpenTutorialButton.OnOpenTutorialRequested += ActivePanel;
    }

    private void OnDisable()
    {
        OpenTutorialButton.OnOpenTutorialRequested -= ActivePanel;
    }

    private void ActivePanel()
    {
        gameInput.ActivePanelMode();

        wrapper.SetActive(true);
        OnPanelActive?.Invoke(true);
    }

    // x ボタンに付与する関数
    public void InactivePanel()
    {
        gameInput.InactivePanelMode();

        wrapper.SetActive(false);
        OnPanelActive?.Invoke(false);
    }

    // 進むボタン
    public void NextPage()
    {
        if (currentPageIndex < pages.Length - 1)
        {
            currentPageIndex++;
            UpdatePageDisplay();
        }
    }

    // 戻るボタン
    public void PrevPage()
    {
        // 最初のページではない場合のみインデックスを減らす
        if (currentPageIndex > 0)
        {
            currentPageIndex--;
            UpdatePageDisplay();
        }
    }

    // チュートリアルUI更新
    private void UpdatePageDisplay()
    {
        for (int i = 0; i < pages.Length; i++)
        {
            if (pages[i] != null)
            {
                // 現在のインデックスと一致するページのみ有効化する
                pages[i].SetActive(i == currentPageIndex);
            }
        }
    }


}
