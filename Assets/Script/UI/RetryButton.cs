using System;
using UnityEngine;

public class RetryButton : MonoBehaviour
{
    public static event Action OnRetryRequested;

    // UIのボタンクリックに割り当てる関数
    public void ClickRetry()
    {
        OnRetryRequested?.Invoke();
    }

}
