using UnityEngine;


[CreateAssetMenu(menuName = "AbilityCondition/RequierLineOfSightCondition")]
public class RequierLineOfSightCondition : ScriptableAbilityCondition
{
    private bool canSeeTarget;

    public override bool IsMet(Enemy enemy)
    {
        //Debug.Log($"Checking if enemy's health is below {lifeThresholdPercent}% of max health.{enemy.CurrentHealth}");
        return enemy.CanSeeTarget(enemy.Target);
    }
}
