using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SpellCharge
{
    /// <summary>
    ///
    /// SpellCharge.IncreaseSpellCount();
    /// Debug.Log(SpellCharge.SpellCount);
    /// 
    /// </summary>
    public static int maxFireCharge = 3;

    public static int SpellCount { get; private set; }

    public static void IncreaseSpellCount(int percentChance)
    {
        int randomChance = Random.Range(0, 101);

        if (randomChance <= percentChance && SpellCount < maxFireCharge)
        {
            SpellCount++;
        }

        UpdateUIFireCharge();
    }

    public static void DecreaseSpellCount()
    {
        SpellCount--;
        UpdateUIFireCharge();
    }

    public static void ResetSpellCount()
    {
        SpellCount = 0;
        UpdateUIFireCharge();
    }

    private static void UpdateUIFireCharge()
    {
        EventManager.FireChargeCountChange(SpellCount);
    }
}
