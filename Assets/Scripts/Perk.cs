using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Perk", menuName = "Perks/Perk")]
public class Perk : ScriptableObject
{
    public string perkName;
    [TextArea] public string description;
    public List<PerkEffect> effects = new List<PerkEffect>();

    // Method to apply the effects of the perk to a spell
    public void ApplyEffects(AttackAndPowerCasting spell)
    {
        foreach (PerkEffect effect in effects)
        {
            effect.ApplyEffect(spell);
        }
    }
}
