using UnityEngine;

[CreateAssetMenu(fileName = "New PlayerSpeedPerk Effect", menuName = "Perks/Effects/PlayerSpeedPerk")]
public class PlayerSpeedPerk : PerkEffect
{
    public override void ApplyEffect()
    {
        PlayerMovement playerMovement = PerksManager.Instance.player.GetComponent<PlayerMovement>();

        //playerMovement.IncreasePlayerMaxMovespeed(10);
        //playerMovement.CurrentPlayerSpeed = playerMovement.PlayerSpeed;
        playerMovement.AddSpeedModifier("Perks", 10);

    }
}
