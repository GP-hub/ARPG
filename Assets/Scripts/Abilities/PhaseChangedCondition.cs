using UnityEngine;

[CreateAssetMenu(menuName = "AbilityCondition/PhaseChangedCondition")]
public class PhaseChangedCondition : ScriptableAbilityCondition
{
    public override bool IsMet(Enemy enemy)
    {
        return enemy.hasPhaseJustChanged;
    }
}