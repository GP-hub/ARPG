using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.HID;
using UnityEngine.InputSystem.XR;

[RequireComponent(typeof(NavMeshObstacle))]
[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : MonoBehaviour
{
    [SerializeField] private float health, maxHealth = 30;
    [SerializeField] private float attackRange;

    [Space(10)]
    [Header("Healthbar")]
    [SerializeField] private GameObject healthBarPrefab;
    [SerializeField] private RectTransform healthPanelRect;

    [Space(10)]
    [Header("Ranged Attack")]
    [SerializeField] private GameObject exitPoint;
    [SerializeField] private GameObject fireballPrefab;
    [SerializeField] private GameObject fireballExplosionPrefab;
    [SerializeField] private float attackProjectileSpeed;
    private int maxObjectsForPooling = 5;


    private List<GameObject> fireballObjectPool = new List<GameObject>();

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

        PoolingFireballObject();
    }


    private void LateUpdate()
    {
        HandleAttack();
        HandleAnimation();
        HandleNavMeshAgentObstacle();
    }

    public void TakeDamage(float damageAmount)
    {
        health -= damageAmount;

        if (health <= 0)
        {
            Destroy(gameObject);
        }
        healthBar.OnHealthChanged(health / maxHealth);
        //Debug.Log("Enemy hp: " + health);
    }

    void HandleAttack()
    {
        if (obstacle.enabled && !agent.enabled)
        {
            float distanceToTarget = Vector3.Distance(transform.position, target.position);

            if (distanceToTarget < attackRange) 
            {
                animator.SetBool("IsAttacking", true);

                LookTowards();

                animator.SetBool("IsWalking", false);
                animator.SetBool("IsIdle", false);
            }
            else
            {
                animator.SetBool("IsAttacking", false);
            }
        }
        else if (!obstacle.enabled && agent.enabled)
        {
            animator.SetBool("IsAttacking", false);
        }
    }

    void LookTowards()
    {
        // Check if the target is valid (not null)
        if (target != null)
        {
            // Calculate the direction from this GameObject to the target
            Vector3 directionToTarget = target.position - transform.position;

            // Create a rotation to look in that direction
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);

            // Smoothly interpolate the current rotation towards the target rotation
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 5f * Time.deltaTime);

        }
    }

    public void MeleeHit()
    {
        Debug.Log("MeleeHit");

        // Range of the sphere of the melee hit
        Collider[] colliders = Physics.OverlapSphere(transform.position, 5f);

        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Player"))
            {
                Debug.Log("Melee hit player");
            }
        }
    }

    public void RangedHit()
    {
        Vector3 targetCorrectedPosition = target.transform.position;
        Vector3 direction = (targetCorrectedPosition - this.transform.position).normalized;

        GameObject newObject = GetPooledFireballObject();
        if (newObject != null)
        {
            newObject.transform.position = exitPoint.transform.position;
            newObject.SetActive(true);
            Rigidbody newObjectRigidbody = newObject.GetComponent<Rigidbody>();
            if (newObjectRigidbody != null)
            {
                newObjectRigidbody.velocity = direction * attackProjectileSpeed;
            }
        }
    }

    void HandleAnimation()
    {
        if (animator.GetBool("IsAttacking")) return;

        float currentSpeed = agent.velocity.magnitude;

        if (currentSpeed > 0.1f)
        {
            animator.SetBool("IsWalking", true);
            animator.SetBool("IsIdle", false);
        }
        else
        {
            animator.SetBool("IsIdle", true);
            animator.SetBool("IsWalking", false);
        }
    }


    void HandleNavMeshAgentObstacle()
    {
        if (target != null)
        {
            float distance = Vector3.Distance(transform.position, target.position);

            //if (distance > agent.stoppingDistance)
            if (distance > attackRange)
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

        agent.SetDestination(AdjustTargetPosition(target));

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

    Vector3 AdjustTargetPosition(Transform transform)
    {
        Vector3 vectorAB = transform.position - this.transform.position;

        Vector3 direction = vectorAB.normalized;

        // Small offset to solve enemy not being close enough not matter the stopping distance .5f
        Vector3 targetPosition = transform.position + direction * attackRange;

        return targetPosition;
    }


    private void PoolingFireballObject()
    {
        for (int i = 0; i < maxObjectsForPooling; i++)
        {
            GameObject newFireballObject = Instantiate(fireballPrefab, Vector3.zero, Quaternion.identity);
            newFireballObject.SetActive(false);
            fireballObjectPool.Add(newFireballObject);
        }
    }

    private GameObject GetPooledFireballObject()
    {
        for (int i = 0; i < fireballObjectPool.Count; i++)
        {
            if (!fireballObjectPool[i].activeInHierarchy)
            {
                return fireballObjectPool[i];
            }
        }

        if (fireballObjectPool.Count < maxObjectsForPooling)
        {
            GameObject newObject = Instantiate(fireballPrefab, exitPoint.transform.position, Quaternion.identity);
            newObject.SetActive(false);
            fireballObjectPool.Add(newObject);
            return newObject;
        }
        return null;
    }
}
