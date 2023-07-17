using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float health, maxHealth = 30;

    [SerializeField] private GameObject healthBarPrefab;
    [SerializeField] private RectTransform healthPanelRect;

    private Healthbar healthBar;

    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
        GeneratePlayerHealthBar(this.gameObject.transform);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(float damageAmount)
    {
        health -= damageAmount;

        if (health <= 0)
        {
            Destroy(gameObject);
        }
        healthBar.OnHealthChanged(health / maxHealth);
        Debug.Log("enemy health: " + health);
    }

    private void GeneratePlayerHealthBar(Transform player)
    {
        GameObject healthBarGo = Instantiate(healthBarPrefab);
        healthBar = healthBarGo.GetComponent<Healthbar>();
        healthBar.SetHealthBarData(player.transform.position + new Vector3(0, 1.8f, 0), healthPanelRect);
        healthBar.transform.SetParent(healthPanelRect, false);
    }
}
