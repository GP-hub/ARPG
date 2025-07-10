using System.Collections.Generic;
using UnityEngine;

public static class SpellCharge
{
    public static int fireballBaseProcChance = 12; // 12.5% approximated as 12
    public static int currentFireballProcChance = fireballBaseProcChance;
    private static int ultimateBuffCount = 0;
    private static bool isBuffedUltimate => ultimateBuffCount > 0;

    public static List<int> bonusProbabilities = new List<int>();

    public static int maxFireCharge = 3;
    public static int SpellCount { get; private set; }

    public static int CalculateTotalChance()
    {
        int total = currentFireballProcChance;

        foreach (int bonus in bonusProbabilities)
            total += bonus;

        return Mathf.Min(total, 100); // cap at 100%
    }

    public static void IncreaseSpellCount()
    {
        if (SpellCount < maxFireCharge)
            SpellCount++;

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
        ResetProcChance();
    }

    public static void ResetBonusProbabilities()
    {
        bonusProbabilities.Clear();
    }

    public static void ResetProcChance()
    {
        currentFireballProcChance = fireballBaseProcChance;
    }

    private static void UpdateUIFireCharge()
    {
        EventManager.FireChargeCountChange(SpellCount);
    }

    public static void TryGainCharge()
    {
        if (isBuffedUltimate)
        {
            IncreaseSpellCount();
            return;
        }
        int random = Random.Range(0, 101);
        int totalChance = CalculateTotalChance();

        if (random <= totalChance)
        {
            IncreaseSpellCount();
            ResetProcChance(); // success → reset
            return;
        }
        else
        {
            currentFireballProcChance = Mathf.Min(currentFireballProcChance * 2, 100); // fail → double chance
            return;
        }
    }

    public static void BuffByUltimate()
    {
        ultimateBuffCount++;
    }
    public static void RemoveUltimateBuff()
    {
        ultimateBuffCount = Mathf.Max(0, ultimateBuffCount - 1);
    }
}
