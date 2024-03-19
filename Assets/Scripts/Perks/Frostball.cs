using UnityEditor.Experimental.GraphView;
using UnityEngine;

[CreateAssetMenu(fileName = "New Frostball Effect", menuName = "Perks/Effects/Frostball")]
public class Frostball : PerkEffect
{

    public override void ApplyEffect()
    {
        AttackAndPowerCasting playerSpell = PerksManager.Instance.player.GetComponent<AttackAndPowerCasting>();

        playerSpell.AttackDamage += 50f;
    }
}
