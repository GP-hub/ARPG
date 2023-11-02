using System.Collections.Generic;
using Unity.VisualScripting;

public static class PlayerStats
{
    public static int playerHealth = 100;
    public static int playerMana;
    public static int fireballBaseProcChance = 0;
    public static List<int> bonusProbabilities = new List<int>();

    // Add a method to calculate the total critical chance.
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

    public static void DamagePlayer(int damage)
    {
        playerHealth -= damage;
    }
}
