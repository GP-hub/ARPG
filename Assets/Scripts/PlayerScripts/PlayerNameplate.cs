using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerNameplate : MonoBehaviour
{

    [Header("Healthbar")]
    [SerializeField] private GameObject healthBarPrefab;
    [SerializeField] private RectTransform healthPanelRect;
    [SerializeField] private Transform hpBarProxyFollow;
    private Healthbar healthBar;
    private GameObject healthBarGo;

    // Start is called before the first frame update
    void Start()
    {
    }

    private void OnEnable()
    {
        GeneratePlayerHealthBar(hpBarProxyFollow);
        InitializeStatusEffectUIBar();
    }

    private void GeneratePlayerHealthBar(Transform hpBarProxy)
    {
        healthBarGo = Instantiate(healthBarPrefab);
        healthBar = healthBarGo.GetComponent<Healthbar>();
        healthBar.SubscribeToFireChargeChange();
        healthBar.SetHealthBarData(hpBarProxy, healthPanelRect);
        healthBar.transform.SetParent(healthPanelRect, false);
    }

    public void UpdateHealthUI(float health, float maxHealth)
    {
        healthBar.OnHealthChanged(health / maxHealth);
    }

    private void InitializeStatusEffectUIBar()
    {
        StatusEffectManager statusManager = GetComponent<StatusEffectManager>();
        StatusEffectUI statusUI = healthBarGo.GetComponentInChildren<StatusEffectUI>();

        statusUI.Initialize(statusManager);
    }
}
