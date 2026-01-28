using UnityEngine;

public class BaseGoalTrigger : MonoBehaviour
{
    [SerializeField] private MonoBehaviour lifeProvider;
    private ILife life;

    private void Awake()
    {
        life = lifeProvider as ILife;
        if (life == null)
        {
            Debug.LogError("lifeProvider には ILifeが必須です。");
            enabled = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!enabled)
            return;

        if (!other.TryGetComponent<Enemy>(out var enemy))
            return;

        life.Damage(1); // 1固定だが、敵ごとに調整するならEnemyStatusから値を引っ張る。
    }

}
