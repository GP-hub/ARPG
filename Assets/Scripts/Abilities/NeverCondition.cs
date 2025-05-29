using UnityEngine;

[CreateAssetMenu(menuName = "AbilityCondition/NeverCondition")]
public class NeverCondition : ScriptableAbilityCondition
{
    public override bool IsMet(Enemy enemy)
    {
        return false;
    }
}
