using UnityEngine;

public class EnemyStatus : MonoBehaviour
{
    public Status maxHp;
    public Status defense;
    public Status speed;
    public Status money;

    public float GetMaxHp()
    {
        return maxHp.GetValue();
    }

    public float GetDefense()
    {
        return defense.GetValue();
    }

    public float GetSpeed()
    {
        return speed.GetValue();
    }

    public int GetMoney()
    {
        return money.GetIntValue();
    }


}
