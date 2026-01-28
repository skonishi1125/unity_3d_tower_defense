using System;
using UnityEngine;

public class LifeManager : MonoBehaviour, ILife
{
    [Header("Initial Values")]
    [SerializeField] private int initialLife = 10;
    [SerializeField] private int life { get; set; }
    public int CurrentLife => life;

    public event Action<int> LifeChanged;


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

        if (life == 0)
        {
            // ゲームオーバー処理
            // GameOver の通知先は「GameState管理」に委ねるのが分離としては綺麗
            // まずはここで何か呼ぶならイベントにするのが無難
        }
    }

    public void Heal(int amount)
    {
        if (amount <= 0) return;

        life += amount;
        LifeChanged?.Invoke(life);
    }
}
