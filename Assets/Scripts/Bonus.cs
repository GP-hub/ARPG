using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bonus : MonoBehaviour
{
    [SerializeField] private KeyCode interactKey = KeyCode.E;
    [SerializeField] private Perk perk;
    private bool playerInRange = false;

    private void Update()
    {
        // Check if the player presses the interactKey
        if (Input.GetKeyDown(interactKey) && playerInRange)
        {
            // Call the SelectPerk method from PerksManager
            PerksManager.Instance.SelectPerk(perk);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the collider belongs to the player
        if (other.CompareTag("Player"))
        {
            // Set the flag to true when the player enters the collider
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Check if the collider belongs to the player
        if (other.CompareTag("Player"))
        {
            // Set the flag to false when the player exits the collider
            playerInRange = false;
        }
    }
}
