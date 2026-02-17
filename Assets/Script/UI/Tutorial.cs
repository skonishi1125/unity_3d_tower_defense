using UnityEngine;

public class Tutorial : MonoBehaviour
{
    [SerializeField] private GameObject wrapper; // UI全体

    private void Awake()
    {
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
        Debug.Log("active des");

        wrapper.SetActive(true);
    }

    // x ボタンに付与する関数
    public void InactivePanel()
    {
        wrapper.SetActive(false);
    }


}
