using UnityEngine;

[CreateAssetMenu(menuName = "AbilityCondition/HealthBelow")]
public class HealthBelowCondition : ScriptableAbilityCondition
{
    public float lifeThresholdPercent = 50f;

    public override bool IsMet(Enemy enemy)
    {
        //Debug.Log($"Checking if enemy's health is below {lifeThresholdPercent}% of max health.{enemy.CurrentHealth}");
        return enemy.CurrentHealth <= (lifeThresholdPercent / 100) * enemy.MaxHealth;
    }
}
