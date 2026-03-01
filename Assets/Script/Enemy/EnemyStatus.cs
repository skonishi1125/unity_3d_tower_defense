using UnityEngine;

public enum EnemyType
{
    Normal,
    Sky,
    Metal
}

public class EnemyStatus : MonoBehaviour
{
    public Status maxHp;
    public Status defense;
    public Status speed;
    public Status money;
    public EnemyType enemyType = EnemyType.Normal;

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

    public bool IsMetal() => enemyType == EnemyType.Metal;

    public bool IsSky() => enemyType == EnemyType.Sky;


}
