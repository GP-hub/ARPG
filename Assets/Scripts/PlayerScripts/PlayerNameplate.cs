using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerNameplate : MonoBehaviour
{

    [Header("Healthbar")]
    [SerializeField] private GameObject healthBarPrefab;
    [SerializeField] private RectTransform healthPanelRect;
    [SerializeField] private Transform hpBarProxyFollow;
    private Healthbar healthBar;

    // Start is called before the first frame update
    void Start()
    {
    }

    private void OnEnable()
    {
        GeneratePlayerHealthBar(hpBarProxyFollow);
        
    }

    private void GeneratePlayerHealthBar(Transform hpBarProxy)
    {
        GameObject healthBarGo = Instantiate(healthBarPrefab);
        healthBar = healthBarGo.GetComponent<Healthbar>();
        healthBar.SubscribeToFireChargeChange();
        healthBar.SetHealthBarData(hpBarProxy, healthPanelRect);
        healthBar.transform.SetParent(healthPanelRect, false);
    }

    public void UpdateHealthUI(float health, float maxHealth)
    {
        healthBar.OnHealthChanged(health / maxHealth);
    }
}
