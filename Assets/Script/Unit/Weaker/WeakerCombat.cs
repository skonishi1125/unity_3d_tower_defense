using UnityEngine;

public class WeakerCombat : UnitCombat
{
    // 通常、最も移動した敵を攻撃するが、
    // Weakerは最も体力の低い敵（倒しやすい敵）を対象とする
    // 最大HP - 現在のHP の結果が一番大きい割合の敵を対象にする。
    // 敵A: 1000 現在 900 | 1 - ( 9 / 10) = 1/10 = 10%
    // 敵B: 50   現在 5   | 1 0 ( 5 / 50) = 9/10 = 90%
    // なので、この場合は敵Bを狙う（実際体力が削れているのはAの方だが、Bのほうが倒しやすいと判断させる）
    protected override void DetectAttackTarget(Enemy enemy, EnemyHealth enemyHealth, ref EnemyHealth attackEnemyHealth, ref float score)
    {
        float hpRatio = 1 - (enemyHealth.CurrentHp / enemyHealth.MaxHp);

        if (attackEnemyHealth == null || hpRatio > score)
        {
            attackEnemyHealth = enemyHealth;
            score = hpRatio;
        }


    }
}
