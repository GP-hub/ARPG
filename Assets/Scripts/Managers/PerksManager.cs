using System.Collections.Generic;
using UnityEngine;

public class PerksManager : Singleton<PerksManager>
{
    public List<Perk> availablePerks = new List<Perk>();
    private List<Perk> selectedPerks = new List<Perk>(); 

    [HideInInspector]public GameObject player;

    private void Start()
    {
        player = GameObject.FindWithTag("Player");
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
        if (player != null)
        {
            perk.ApplyEffects();
        }
    }
}
