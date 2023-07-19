using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.XR;

[RequireComponent(typeof(NavMeshObstacle))]
[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : MonoBehaviour
{
    [SerializeField] private float health, maxHealth = 30;

    [Space(10)]
    [Header("Healthbar")]
    [SerializeField] private GameObject healthBarPrefab;
    [SerializeField] private RectTransform healthPanelRect;

    private NavMeshAgent agent;
    private NavMeshObstacle obstacle;

    private Healthbar healthBar;

    private Transform target;

    private bool isCoroutineRunning;
    private bool isCoroutineRunningTwo;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        obstacle = GetComponent<NavMeshObstacle>();

        obstacle.enabled = false;
        obstacle.carveOnlyStationary = false;
        obstacle.carving = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
        GeneratePlayerHealthBar(this.gameObject);
        target = GameObject.Find("Player").transform;
    }


    private void LateUpdate()
    {
        HandleNavMeshAgentObstacleTwo();
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


    private void HandleNavMeshAgentObstacleTwo()
    {
        if (target != null)
        {
            float distance = Vector3.Distance(transform.position, target.position);

            if (distance > 2f)
            {
                // Enable NavMeshAgent and disable NavMeshObstacle
                obstacle.enabled = false;

                if (!isCoroutineRunning)
                {
                    StartCoroutine(WaitForAgentEnabling());
                }
            }
            else
            {
                
                // Disable NavMeshAgent and enable NavMeshObstacle
                agent.enabled = false;

                if (!isCoroutineRunningTwo)
                {
                    StartCoroutine(WaitForObstacleEnabling());
                }
            }
        }
    }

    private void GeneratePlayerHealthBar(GameObject player)
    {
        GameObject healthBarGo = Instantiate(healthBarPrefab);
        healthBar = healthBarGo.GetComponent<Healthbar>();
        healthBar.SetHealthBarData(player.transform, healthPanelRect);
        healthBar.transform.SetParent(healthPanelRect, false);
    }

    IEnumerator WaitForAgentEnabling()
    {
        StopCoroutine(WaitForObstacleEnabling());
        isCoroutineRunning = true;

        yield return new WaitForSeconds(.1f);
        agent.enabled = true;

        agent.SetDestination(target.position);

        isCoroutineRunning = false;
    }

    IEnumerator WaitForObstacleEnabling()
    {
        StopCoroutine(WaitForAgentEnabling());

        isCoroutineRunningTwo = true;

        yield return null;

        obstacle.enabled = true;

        isCoroutineRunningTwo = false;
    }

}
