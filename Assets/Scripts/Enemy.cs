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
    private Animator animator;
    private Rigidbody rb;

    private Healthbar healthBar;

    private Transform target;

    private bool isCoroutineAgentEnabling;
    private bool isCoroutineObstacleEnabling;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        obstacle = GetComponent<NavMeshObstacle>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

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
        HandleAttack();
        HandleAnimation();
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

    void HandleAttack()
    {
        if (true)
        {

        }
    }


    void HandleAnimation()
    {
        float currentSpeed = agent.velocity.magnitude;
        Debug.Log("enemy speed: " + currentSpeed);

        //if (currentSpeed > 0.1f)
        //{
        //    animator.SetBool("IsWalking", true);
        //    animator.SetBool("IsIdle", false);
        //}
        //else
        //{
        //    animator.SetBool("IsIdle", false);
        //    animator.SetBool("IsWalking", true);
        //}
    }


    void HandleNavMeshAgentObstacleTwo()
    {
        if (target != null)
        {
            float distance = Vector3.Distance(transform.position, target.position);

            if (distance > agent.stoppingDistance)
            {
                // Enable NavMeshAgent and disable NavMeshObstacle
                obstacle.enabled = false;

                if (!isCoroutineAgentEnabling)
                {
                    StartCoroutine(WaitForAgentEnabling());
                }
            }
            else
            {
                
                // Disable NavMeshAgent and enable NavMeshObstacle
                agent.enabled = false;

                if (!isCoroutineObstacleEnabling)
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
        isCoroutineAgentEnabling = true;

        yield return new WaitForSeconds(.1f);
        agent.enabled = true;

        agent.SetDestination(target.position);

        isCoroutineAgentEnabling = false;
    }

    IEnumerator WaitForObstacleEnabling()
    {
        StopCoroutine(WaitForAgentEnabling());

        isCoroutineObstacleEnabling = true;

        yield return null;

        obstacle.enabled = true;

        isCoroutineObstacleEnabling = false;
    }

}
