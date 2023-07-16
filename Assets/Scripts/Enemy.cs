using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] float health, maxHealth = 30;

    //public GameObject playerPrefab;
    public GameObject healthBarPrefab;
    //public Transform playersParent;
    public RectTransform healthPanelRect;

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
        Debug.Log("enemy health: " + health);
    }

    private void GeneratePlayerHealthBar(Transform player)
    {
        GameObject healthBar = Instantiate(healthBarPrefab) as GameObject;
        healthBar.GetComponent<Healthbar>().SetHealthBarData(player.transform.position + new Vector3(0, 1.8f, 0), healthPanelRect);
        healthBar.transform.SetParent(healthPanelRect, false);
    }
}
