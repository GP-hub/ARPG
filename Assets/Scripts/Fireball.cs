using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Fireball : MonoBehaviour
{
    public GameObject projectilePrefab; // The prefab for the projectile
    public Transform firePoint; // The point from which the projectile will be fired

    public float projectileSpeed = 10f; // The speed of the projectile
    public float critChance = 0.1f; // The chance of the spell critting (10% in this example)
    public float cooldownTime = 1f;

    private bool isCooldown;
    private Coroutine castSpellCoroutine;

    private PlayerControls controls; // Reference to the Input System controls

    private void Awake()
    {
        controls = new PlayerControls();

        controls.Controls.Attack.started += _ => StartCastingSpell();
        controls.Controls.Attack.canceled += _ => StopCastingSpell();
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    private void StartCastingSpell()
    {
        // Start the coroutine that casts the spell repeatedly while Fire is held down
        castSpellCoroutine = StartCoroutine(CastSpellRepeatedly());
    }

    private void StopCastingSpell()
    {
        // Stop the coroutine that casts the spell repeatedly
        if (castSpellCoroutine != null)
        {
            StopCoroutine(castSpellCoroutine);
        }
    }

    private IEnumerator CastSpellRepeatedly()
    {
        while (true)
        {
            // Check if the spell is on cooldown
            if (isCooldown)
            {
                Debug.Log("The spell is on cooldown!");
            }
            else
            {
                Debug.Log("Casting spell!");

                // Instantiate the projectile prefab at the fire point
                GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);

                // Move the projectile using transform.position instead of Rigidbody2D.velocity
                float step = projectileSpeed * Time.deltaTime;
                while (projectile != null)
                {
                    projectile.transform.position += projectile.transform.right * step;
                    yield return null;
                }

                // Check if the spell crits
                float randomValue = Random.value; // Generate a random value between 0 and 1
                if (randomValue < critChance)
                {
                    Debug.Log("The spell crits!");
                    // Do something special for the crit
                }

                // Set the spell on cooldown
                isCooldown = true;
                StartCoroutine(Cooldown());
            }

            // Wait for a short time before trying to cast the spell again
            yield return new WaitForSeconds(0.1f);
        }
    }

    private IEnumerator Cooldown()
    {
        // Wait for the cooldown time
        yield return new WaitForSeconds(cooldownTime);
        // Reset the spell cooldown
        isCooldown = false;
    }
}
