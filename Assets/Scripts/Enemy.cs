using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Rigidbody))]
public class Enemy : MonoBehaviour
{
    [SerializeField] private float health, maxHealth = 30;
    [SerializeField] private float attackRange;
    [SerializeField] private float speed;
    private LayerMask groundLayerMask;

    [Space(10)]
    [Header("Healthbar")]
    [SerializeField] private GameObject healthBarPrefab;
    [SerializeField] private RectTransform healthPanelRect;

    [Space(10)]
    [Header("Attack")]
    [SerializeField] private GameObject exitPoint;
    [SerializeField] private GameObject fireballPrefab;
    [SerializeField] private GameObject fireballExplosionPrefab;
    [SerializeField] private float attackProjectileSpeed;
    [SerializeField] private LayerMask characterLayer;
    [SerializeField] private float meleeHitboxSize;
    private int maxObjectsForPooling = 5;


    private NavMeshAgent agent;

    private List<GameObject> fireballObjectPool = new List<GameObject>();

    private Animator animator;
    private Rigidbody rb;

    private Healthbar healthBar;

    private Transform target;

    Vector3 velocity = Vector3.zero;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
        agent.updatePosition = false;


        groundLayerMask = LayerMask.GetMask("Ground");
    }

    // 
    void Start()
    {
        AIManager.Instance.Units.Add(this);
        health = maxHealth;
        GeneratePlayerHealthBar(this.gameObject);

        // Pass the player as the target for now
        target = GameObject.Find("Player").transform;

        PoolingFireballObject();
    }


    private void FixedUpdate()
    {
        HandleAnimation();
        HandleAttack();
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
        float distanceToTarget = Vector3.Distance(transform.position, target.position);

        if (!CanSeePlayer())
        {
            //Move(target.transform.position);
            AIManager.Instance.MakeAgentCircleTarget(target.transform);
            HandleAttackAnimation(false);
            return;
        }


        if (CanSeePlayer())
        {
            if (distanceToTarget <= attackRange)
            {
                Stop();
                HandleAttackAnimation(true);
            }
            if (distanceToTarget > attackRange)
            {
                //Move(target.transform.position);
                AIManager.Instance.MakeAgentCircleTarget(target.transform);
                HandleAttackAnimation(false);
            }
        }
    }

    void HandleAttackAnimation(bool onRange)
    {
        if (onRange)
        {
            animator.SetBool("IsAttacking", true);
            LookTowards();

            animator.SetBool("IsWalking", false);
            animator.SetBool("IsIdle", false);

        }
        if (!onRange)
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
        // Max number of entities in the OverlapSphere
        int maxColliders = 10;
        Collider[] hitColliders = new Collider[maxColliders];
        int numColliders = Physics.OverlapSphereNonAlloc(transform.position, meleeHitboxSize, hitColliders, characterLayer);

        for (int i = 0; i < numColliders; i++)
        {
            if (hitColliders[i].CompareTag("Player"))
            {
                Debug.Log("Damaging player here");
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
            Quaternion rotationToTarget = Quaternion.LookRotation(direction);
            newObject.transform.rotation = rotationToTarget;
            //Rigidbody newObjectRigidbody = newObject.GetComponent<Rigidbody>();
            //if (newObjectRigidbody != null)
            //{
            //    newObjectRigidbody.velocity = direction * attackProjectileSpeed;
            //}
        }
    }

    void HandleAnimation()
    {
        if (animator.GetBool("IsAttacking")) return;

        float currentSpeedRb = rb.velocity.magnitude;

        float currentSpeed = agent.velocity.magnitude;
        //Debug.Log("Agent: " + this.gameObject.name + " , agent speed: " + currentSpeed + " , Rb speed: " + currentSpeedRb);
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

    // Directly moving towards the player
    public void Move(Vector3 targetPos)
    {
        agent.avoidancePriority = 50;

        agent.isStopped = false;
        agent.autoRepath = true;

        if (agent.speed == 0)
        {
            agent.speed = speed;
        }
        agent.SetDestination(targetPos);

        // Calculate the new position using SmoothDamp logic
        Vector3 smoothDampedPosition = Vector3.SmoothDamp(transform.position, agent.nextPosition, ref velocity, 0.1f);

        // Calculate the direction and distance to move
        Vector3 moveDelta = smoothDampedPosition - transform.position;

        // Use the NavMeshAgent's MovePosition method to move the agent
        rb.MovePosition(transform.position + moveDelta);
    }

    // Moving around the target via AIManager
    public void MoveAIUnit(Vector3 targetPos)
    {
        agent.avoidancePriority = 50;

        agent.isStopped = false;
        agent.autoRepath = true;

        if (agent.speed == 0)
        {
            agent.speed = speed;
        }
        agent.SetDestination(targetPos);

        // Calculate the new position using SmoothDamp logic
        Vector3 smoothDampedPosition = Vector3.SmoothDamp(transform.position, agent.nextPosition, ref velocity, 0.1f);

        // Calculate the direction and distance to move
        Vector3 moveDelta = smoothDampedPosition - transform.position;

        // Use the NavMeshAgent's MovePosition method to move the agent
        rb.MovePosition(transform.position + moveDelta);
    }

    void Stop()
    {
        agent.isStopped = true;
        agent.avoidancePriority = 2;

        agent.ResetPath();
    }

    private void GeneratePlayerHealthBar(GameObject player)
    {
        GameObject healthBarGo = Instantiate(healthBarPrefab);
        healthBar = healthBarGo.GetComponent<Healthbar>();
        healthBar.SetHealthBarData(player.transform, healthPanelRect);
        healthBar.transform.SetParent(healthPanelRect, false);
    }

    // If we want to move to a position close to the target instead of the target exact position
    Vector3 AdjustTargetPosition(Transform transform)
    {
        Vector3 vectorAB = transform.position - this.transform.position;

        Vector3 direction = vectorAB.normalized;

        // Small offset to solve enemy not being close enough not matter the stopping distance .5f
        Vector3 targetPosition = transform.position + direction * .5f;

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

    private bool CanSeePlayer()
    {
        // Direction from the enemy to the player
        Vector3 directionToPlayer = (target.position + new Vector3(0f, 1f, 0f)) - (transform.position + new Vector3(0f, 1f, 0f));

        // Draw a debug ray to visualize the raycast in the scene view
        Debug.DrawRay(transform.position + new Vector3(0f, 1f, 0f), directionToPlayer, Color.blue);

        // Check if there's a clear line of sight by performing a raycast from the enemy's position to the player's position
        //if (Physics.Raycast(transform.position + new Vector3(0f, 1f, 0f), directionToPlayer, out RaycastHit hit, 100, ~groundLayerMask))
        if (Physics.Raycast(transform.position + new Vector3(0f, 1f, 0f), directionToPlayer, out RaycastHit hit, 100, LayerMask.GetMask("Character", "Obstacle")))
        {
            if (hit.collider.CompareTag("Player"))
            {
                return true;
            }
        }

        return false;
    }
}
