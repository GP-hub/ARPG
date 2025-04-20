[System.Serializable]
public class Condition : IAbilityCondition
{
    public bool IsMet(Enemy enemy)
    {
        return true;
    }
}