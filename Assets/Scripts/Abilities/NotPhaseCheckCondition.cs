using UnityEngine;

[CreateAssetMenu(menuName = "AbilityCondition/NotPhaseCheckCondition")]
public class NotPhaseCheckCondition : ScriptableAbilityCondition
{
    public int phaseToExlude;

    public override bool IsMet(Enemy enemy)
    {
        return enemy.CurrentPhase != phaseToExlude;
    }
}