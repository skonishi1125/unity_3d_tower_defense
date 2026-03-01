using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackToTitleButton : MonoBehaviour
{
    public static event Action OnBackToTitleRequested;

    public void PrevTitleScene()
    {
        OnBackToTitleRequested?.Invoke();
    }
}
