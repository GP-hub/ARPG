using UnityEngine;

[CreateAssetMenu(menuName = "AbilityCondition/PhaseCheckCondition")]
public class PhaseCheckCondition : ScriptableAbilityCondition
{
    public int phaseNbr;

    public override bool IsMet(Enemy enemy)
    {
        //Debug.Log($"Checking if enemy's health is below {lifeThresholdPercent}% of max health.{enemy.CurrentHealth}");
        return enemy.CurrentPhase == phaseNbr;
    }
}