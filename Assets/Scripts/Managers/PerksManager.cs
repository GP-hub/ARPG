using System.Collections.Generic;
using UnityEngine;

public class PerksManager : Singleton<PerksManager>
{
    public List<Perk> availablePerks = new List<Perk>();
    private List<Perk> selectedPerks = new List<Perk>(); 

    private AttackAndPowerCasting spell;

    private void Start()
    {
        spell = GameObject.FindWithTag("Player").GetComponent<AttackAndPowerCasting>();
    }

    // Method to select a perk
    public void SelectPerk(Perk perk)
    {
        if (!selectedPerks.Contains(perk))
        {
            selectedPerks.Add(perk);
            ApplyPerks(perk);
        }
    }

    // Method to apply perks to spells
    public void ApplyPerks(Perk perk)
    {
        if (spell != null)
        {
            perk.ApplyEffects(spell);
            //foreach (Perk perk in selectedPerks)
            //{
            //}
        }
    }
}
