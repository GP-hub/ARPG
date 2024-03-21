using UnityEngine;

[CreateAssetMenu(fileName = "New PlayerSpeedPerk Effect", menuName = "Perks/Effects/PlayerSpeedPerk")]
public class PlayerSpeedPerk : PerkEffect
{
    public override void ApplyEffect()
    {
        PlayerMovement playerMovement = PerksManager.Instance.player.GetComponent<PlayerMovement>();

        playerMovement.PlayerSpeed += 10f;
    }
}
