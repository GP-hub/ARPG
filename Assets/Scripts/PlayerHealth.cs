using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private float health, maxHealth = 100;

    private PlayerNameplate playerNameplate;
    private Animator animator;
    private bool isAlive = true;

    public bool IsAlive { get => isAlive; }

    //
    void Start()
    {
        playerNameplate = GetComponent<PlayerNameplate>();
        animator = GetComponent<Animator>();
        health = maxHealth;
        EventManager.onPlayerTakeDamage += PlayerTakeDamage;
        EventManager.onPlayerTakeHeal += PlayerTakeHeal;
    }

    public void PlayerTakeDamage(int damageAmount)
    {
        health -= damageAmount;

        if (health > maxHealth) health = maxHealth;

        if (health <= 0) Death();

        playerNameplate.UpdateHealthUI(health, maxHealth);
    }

    public void PlayerTakeHeal(int healAmount)
    {
        health += healAmount;

        if (health > maxHealth) health = maxHealth;

        if (health <= 0) Death();

        playerNameplate.UpdateHealthUI(health, maxHealth);
    }

    private void Death()
    {
        this.gameObject.tag = "Dead";
        isAlive = false;
        animator.SetTrigger("Death");
    }
}
