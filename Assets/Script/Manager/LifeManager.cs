using System;
using UnityEngine;

public class LifeManager : MonoBehaviour, ILife
{
    [Header("Initial Values")]
    [SerializeField] private int initialLife = 10;
    private int life;
    private bool isLifeZero = false;

    public int CurrentLife => life;

    public event Action<int> LifeChanged;
    public event Action LifeZero;

    private void Awake()
    {
        life = initialLife;
    }

    private void Start()
    {
        LifeChanged?.Invoke(life);
    }

    public void Damage(int amount)
    {
        if (amount <= 0)
            return;

        life = Mathf.Max(0, life - amount);
        LifeChanged?.Invoke(life);

        if (life == 0 && ! isLifeZero)
        {
            isLifeZero = true;
            LifeZero?.Invoke();
        }
    }

    public void Heal(int amount)
    {
        if (amount <= 0) return;

        life += amount;
        LifeChanged?.Invoke(life);
    }
}
