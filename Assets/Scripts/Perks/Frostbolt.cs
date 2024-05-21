using UnityEngine;

[CreateAssetMenu(fileName = "New Frostbolt Effect", menuName = "Perks/Effects/Frostbolt")]
public class Frostbolt : PerkEffect
{

    public override void ApplyEffect()
    {
        AttackAndPowerCasting playerSpell = PerksManager.Instance.player.GetComponent<AttackAndPowerCasting>();
        playerSpell.FireballPrefabName = "Frostbolt";

    }
}
