using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Perk", menuName = "Perks/Perk")]
public class Perk : ScriptableObject
{
    public string perkName;
    public Sprite perkImage;
    [TextArea] public string perkDescription;
    public List<PerkEffect> effects = new List<PerkEffect>();

    // Method to apply the effects of the perk to a spell
    public void ApplyEffects()
    {
        foreach (PerkEffect effect in effects)
        {
            effect.ApplyEffect();
            Debug.Log("Apply effect: " + effect);
        }
    }
}
