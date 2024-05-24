using UnityEngine;

[CreateAssetMenu(fileName = "New Damage buff Effect", menuName = "Perks/Effects/Damage buff")]
public class DamageBuff : PerkEffect
{

    public override void ApplyEffect()
    {
        AttackAndPowerCasting playerSpell = PerksManager.Instance.player.GetComponent<AttackAndPowerCasting>();

        playerSpell.AttackDamage += 50f;
    }
}
