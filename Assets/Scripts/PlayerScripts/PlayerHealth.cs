using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int health, maxHealth = 100;

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

        EventManager.PlayerUpdateHealthUI(health, maxHealth);
    }

    void OnDestroy()
    {
        EventManager.onPlayerTakeDamage -= PlayerTakeDamage;
        EventManager.onPlayerTakeHeal -= PlayerTakeHeal;
    }

    public void PlayerTakeDamage(int damageAmount)
    {
        health -= damageAmount;
        health = Mathf.Clamp(health, 0, maxHealth);
        if (health > maxHealth) health = maxHealth;

        if (health <= 0) Death();

        playerNameplate.UpdateHealthUI(health, maxHealth);
        EventManager.PlayerUpdateHealthUI(health, maxHealth);

        DamageNumberPool.Instance.ShowDamage(new Vector3(this.transform.position.x, this.transform.position.y+2, this.transform.position.z), "-" + Mathf.CeilToInt(damageAmount).ToString(), Color.red);
    }

    public void PlayerTakeHeal(int healAmount)
    {
        health += healAmount;
        health = Mathf.Clamp(health, 0, maxHealth);
        if (health > maxHealth) health = maxHealth;

        if (health <= 0) Death();

        playerNameplate.UpdateHealthUI(health, maxHealth);
        EventManager.PlayerUpdateHealthUI(health, maxHealth);
        DamageNumberPool.Instance.ShowDamage(new Vector3(this.transform.position.x, this.transform.position.y + 2, this.transform.position.z), "+"+ Mathf.CeilToInt(healAmount).ToString(), Color.green);

    }

    private void Death()
    {
        this.gameObject.tag = "Dead";
        isAlive = false;
        animator.SetTrigger("Death");
        EventManager.PlayerDeath();
    }
}
