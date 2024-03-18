using UnityEngine;

[CreateAssetMenu(fileName = "New Frostball Effect", menuName = "Perks/Effects/Frostball")]
public class Frostball : PerkEffect
{
    public override void ApplyEffect(AttackAndPowerCasting spell)
    {
        spell.AttackDamage += 50f;
    }
}
