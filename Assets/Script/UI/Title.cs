using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class Title : MonoBehaviour
{
    [SerializeField] private string battleSceneName;

    [SerializeField] private Button tutorialButton;
    [SerializeField] private Button startButton;

    private void Awake()
    {
        Debug.Log("awake title");
    }

    // Battleシーンに移動するButtonに割り当てる
    public void ToBattleScene()
    {
        SceneManager.LoadScene(battleSceneName);

    }

}
