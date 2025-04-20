using UnityEngine;

public abstract class ScriptableAbilityCondition : ScriptableObject
{
    public abstract bool IsMet(Enemy enemy);
}
