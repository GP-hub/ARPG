using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private float health, maxHealth = 100;

    private PlayerNameplate playerNameplate;

    //
    void Start()
    {
        playerNameplate = this.GetComponent<PlayerNameplate>();
        health = maxHealth;
        EventManager.Instance.onPlayerTakeDamage += PlayerTakeDamage;
        EventManager.Instance.onPlayerTakeHeal += PlayerTakeHeal;
    }

    public void PlayerTakeDamage(int damageAmount)
    {
        health -= damageAmount;

        if (health <= 0)
        {
            // Do the correct logic to get rid of dead enemies here
            Destroy(gameObject);
        }
        playerNameplate.UpdateHealthUI(health, maxHealth);
        Debug.Log("Player hp: " + health);
    }

    public void PlayerTakeHeal(int healAmount)
    {
        health += healAmount;

        if (health > maxHealth)
        {
            health = maxHealth;
        }

        playerNameplate.UpdateHealthUI(health, maxHealth);
        Debug.Log("Player hp: " + health);
    }
}
