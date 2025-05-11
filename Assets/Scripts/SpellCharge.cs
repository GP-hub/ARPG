using System.Collections.Generic;
using UnityEngine;

public static class SpellCharge
{
    public static int fireballBaseProcChance = 0;
    public static List<int> bonusProbabilities = new List<int>();

    public static int CalculateTotalChance()
    {
        int totalChance = fireballBaseProcChance;

        foreach (int bonus in bonusProbabilities)
        {
            totalChance += bonus;
        }

        return totalChance;
    }

    // Add a method to add a bonus probability to the list.
    public static void AddBonusProbability(int bonus)
    {
        bonusProbabilities.Add(bonus);
    }

    // Add a method to remove a bonus probability from the list.
    public static void RemoveBonusProbability(int bonus)
    {
        bonusProbabilities.Remove(bonus);
    }

    public static void ResetBonusProbabilities()
    {
        bonusProbabilities.Clear();
    }

    public static int maxFireCharge = 3;

    public static int SpellCount { get; private set; }

    public static void IncreaseSpellCount(int percentChance)
    {
        int randomChance = Random.Range(0, 101);

        if (randomChance <= percentChance && SpellCount < maxFireCharge)
        {
            SpellCount = Mathf.Clamp(SpellCount + 1 , 0, maxFireCharge);
        }

        UpdateUIFireCharge();
    }

    public static void DecreaseSpellCount()
    {
        SpellCount = Mathf.Clamp(SpellCount - 1, 0, maxFireCharge);
        UpdateUIFireCharge();
    }

    public static void ResetSpellCount()
    {
        SpellCount = 0;
        UpdateUIFireCharge();
    }

    public static void InitializeSpellCharge()
    {
        ResetBonusProbabilities();
        ResetSpellCount();
    }

    private static void UpdateUIFireCharge()
    {
        EventManager.FireChargeCountChange(SpellCount);
    }
}
