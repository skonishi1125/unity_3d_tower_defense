using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/IntEvent")]
public class IntEventChannelSO : ScriptableObject
{
    public event Action<int> OnEventRaised;

    public void RaiseEvent(int value)
    {
        OnEventRaised?.Invoke(value);
    }
}
