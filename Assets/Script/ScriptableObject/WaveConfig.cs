using System;
using UnityEngine;

[Serializable]
public class EnemyGroup
{
    public GameObject enemyPrefab;
    public int spawnCount; // 何体出すか
    public float spawnInterval; // 何秒おきに出すか
}

// ステータスとか弄りたくなったら、ここでmultiplierを含む実装にすればよいだろう

[CreateAssetMenu(menuName = "Game/WaveConfig")]
public class WaveConfig : ScriptableObject
{
    public string waveName;

    public EnemyGroup[] enemyGroups;
    public float startDelay = 0f;
    public float endDelay = 0f;
    public bool isBossWave;
}
