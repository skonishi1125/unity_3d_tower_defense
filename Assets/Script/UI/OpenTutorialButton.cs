using System;
using UnityEngine;

public class OpenTutorialButton : MonoBehaviour
{
    public static event Action OnOpenTutorialRequested;

    public void ClickOpenTutorial()
    {
        Debug.Log("click tutorial");
        OnOpenTutorialRequested?.Invoke();
    }

}
