using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PreStart : MonoBehaviour
{
    [SerializeField] private GameInput gameInput;
    [SerializeField] private GameObject guide;

    // HudUIのチュートリアルボタン
    [SerializeField] private GameObject tutorialButton;

    public event Action<bool> OnPreStartActive;


    private void Awake()
    {
        if (gameInput == null)
            Debug.LogWarning("Prestart: gameInput未割当");
        if (guide == null)
            Debug.LogWarning("PreStart: ガイド未割当");

        guide.SetActive(true);
    }

    private void Start()
    {
        if (gameInput != null)
            gameInput.ActivePanelMode();

        tutorialButton.SetActive(false);

    }


    // 閉じるボタンに割り当てるメソッド
    public void CloseGuide()
    {
        guide.SetActive(false);
        tutorialButton.SetActive(true);
        gameInput.InactivePanelMode();
        OnPreStartActive?.Invoke(false);

    }

}
