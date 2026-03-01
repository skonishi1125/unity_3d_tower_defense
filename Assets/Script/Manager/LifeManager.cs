using System;
using UnityEngine;

public class LifeManager : MonoBehaviour, ILife
{
    [Header("Initial Values")]
    [SerializeField] private int initialLife = 10;
    private int life;
    private bool isLifeZero = false;

    [Header("SFX")]
    [SerializeField] private AudioClip lifeDamagedSfx;
    [SerializeField] private AudioClip lifeZeroSfx;

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

        if (life == 0 && !isLifeZero)
        {
            isLifeZero = true;
            AudioManager.Instance?.PlaySfx(lifeZeroSfx);
            LifeZero?.Invoke();
        }
        else if (life != 0 && !isLifeZero)
        {
            // Zero以外は、ダメージ音を鳴らす
            AudioManager.Instance?.PlaySfx(lifeDamagedSfx);
        }
    }

    public void Heal(int amount)
    {
        if (amount <= 0) return;

        life += amount;
        LifeChanged?.Invoke(life);
    }

    // ゲームオーバー処理確認用
    [ContextMenu("Debug/Life Zero")]
    private void DebugLifeZero()
    {
        life = 0;
        isLifeZero = true;
        LifeZero?.Invoke();
        Debug.Log("DebugLifeZero 実行");
    }



}
