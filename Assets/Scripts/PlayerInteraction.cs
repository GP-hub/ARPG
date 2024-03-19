using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    public LayerMask interactibleLayer;

    // Interaction distance
    public float interactionRange = 2f;

    // Spherecast hit buffer
    private RaycastHit[] hitsBuffer = new RaycastHit[10]; // Adjust the size according to your needs
    private PlayerInput playerInput;

    private void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        playerInput.actions.FindAction("Interact").performed += Interact;
    }

    private void Interact(InputAction.CallbackContext context)
    {
        // Check for input to trigger interaction
        if (context.performed)
        {
            // Perform a spherecast to find interactible objects within range
            int hitsCount = Physics.SphereCastNonAlloc(transform.position, interactionRange, transform.forward, hitsBuffer, 0f, interactibleLayer);

            // Iterate through all hits
            for (int i = 0; i < hitsCount; i++)
            {
                // Check if the hit object has a script with a 'getbonus' method
                Bonus interactibleObject = hitsBuffer[i].collider.GetComponent<Bonus>();
                if (interactibleObject != null)
                {
                    // Call the 'getbonus' method
                    interactibleObject.GetBonus();
                }
            }
        }
    }
}
