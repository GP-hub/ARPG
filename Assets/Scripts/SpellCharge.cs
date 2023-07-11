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


    public static int SpellCount { get; private set; }

    public static void IncreaseSpellCount()
    {
        SpellCount++;
    }

    public static void DecreaseSpellCount()
    {
        SpellCount--;
    }

    public static void ResetSpellCount()
    {
        SpellCount = 0;
    }
}
