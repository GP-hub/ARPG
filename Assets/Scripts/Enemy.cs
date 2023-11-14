using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UnityEditor.FilePathAttribute;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(CharacterController))]
public class Enemy : MonoBehaviour
{
    [SerializeField] private float health, maxHealth = 30;
    [SerializeField] private float attackRange;
    [Tooltip("Animator issue when speed is below 2")]
    [SerializeField] private float speed;
    // Idk 
    //[SerializeField] public LayerMask mask;

    [Space(10)]
    [Header("Healthbar")]
    [SerializeField] private GameObject healthBarPrefab;
    [SerializeField] private RectTransform healthPanelRect;
    [SerializeField] private Transform hpBarProxyFollow;

    [Space(10)]
    [Header("Attack")]
    [SerializeField] private GameObject exitPoint;
    [SerializeField] private string fireballPrefabName;
    [SerializeField] private string AoePrefabName;
    [SerializeField] private float attackProjectileSpeed;
    [SerializeField] private LayerMask characterLayer;
    [SerializeField] private float meleeHitboxSize;

    private bool isAttacking;
    private NavMeshAgent agent;

    private Animator animator;

    private CharacterController controller;

    private Healthbar healthBar;

    private Transform target;

    Vector3 velocity = Vector3.zero;

    private Vector3 lastPosition;

    private float calculatedSpeed;

    private bool isGrounded = true;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        agent = GetComponent<NavMeshAgent>();
    }

    // 
    void Start()
    {
        agent.speed = speed;
        AIManager.Instance.Units.Add(this);
        health = maxHealth;
        GeneratePlayerHealthBar(hpBarProxyFollow);

        // Pass the player as the target for now
        target = GameObject.Find("Player").transform;

        lastPosition = transform.position;

        StartCoroutine(CheckGroundedStatus());
    }


    private void Update()
    {
        HandleAnimation();
        HandleAttack();
    }


    private IEnumerator CheckGroundedStatus()
    {
        while (true)
        {
            // Check if the character controller is grounded
            isGrounded = controller.isGrounded;

            // Enable/disable the NavMeshAgent based on grounding status
            if (isGrounded) agent.enabled = true;
            else agent.enabled = false;

            // Wait for a short duration before checking again
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void FixedUpdate()
    {
        CurrentSpeed();
    }

    public void TakeDamage(float damageAmount)
    {
        health -= damageAmount;

        if (health <= 0)
        {
            // Do the correct logic to get rid of dead enemies here
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
            if (agent.enabled)
            {
                AIManager.Instance.MakeAgentCircleTarget(target.transform);
                HandleAttackAnimation(false);
                return;
            }
        }


        if (CanSeePlayer())
        {
            if (agent.enabled)
            {
                if (!agent.pathPending && agent.remainingDistance < AIManager.Instance.Radius || attackRange > agent.remainingDistance/* || !agent.pathPending && agent.remainingDistance < attackRange*/) 
                {
                    if (distanceToTarget <= attackRange)
                    {
                        Stop();
                        HandleAttackAnimation(true);
                        return;
                    }
                    if (distanceToTarget > attackRange)
                    {
                        // HERE THE OTHER UNIT WILL THE ONE ALREADY ATTCKING TO MOVE !!!
                        AIManager.Instance.MakeAgentCircleTarget(target.transform);
                        HandleAttackAnimation(false);
                        return;
                    }
                }
            }
        }
    }

    void HandleAttackAnimation(bool onRange)
    {
        if (onRange)
        {
            animator.SetBool("IsAttacking", true);
            isAttacking = true;
            LookTowards();

            animator.SetBool("IsWalking", false);
            animator.SetBool("IsIdle", false);

        }
        if (!onRange)
        {
            animator.SetBool("IsAttacking", false);
            isAttacking = false;
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

    // Triggered via Melee Attack animation
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

    // Triggered via SpawnAoe Attack Animation
    public void SpawnAOE()
    {
        PoolingManagerSingleton.Instance.GetObjectFromPool(AoePrefabName, target.transform.position + new Vector3(0, 0.2f, 0));
    }


    // Triggered via Ranged Attack animation
    public void RangedHit()
    {
        Vector3 targetCorrectedPosition = target.transform.position;
        Vector3 direction = (targetCorrectedPosition - this.transform.position).normalized;

        GameObject newObject = PoolingManagerSingleton.Instance.GetObjectFromPool(fireballPrefabName, exitPoint.transform.position);

        if (newObject != null)
        {
            Quaternion rotationToTarget = Quaternion.LookRotation(direction);
            newObject.transform.rotation = rotationToTarget;
        }
    }

    void HandleAnimation()
    {
        if (calculatedSpeed > 1f)
        {
            animator.SetBool("IsWalking", true);
            animator.SetBool("IsIdle", false);
            animator.SetBool("IsAttacking", false);
            isAttacking = false;
        }
        else
        {
            animator.SetBool("IsIdle", true);
            animator.SetBool("IsWalking", false);
        }
    }

    // Directly moving towards the player
    //public void Move(Vector3 targetPos)
    //{
    //    agent.avoidancePriority = 50;

    //    agent.isStopped = false;
    //    agent.autoRepath = true;

    //    if (agent.speed == 0)
    //    {
    //        agent.speed = speed;
    //    }
    //    agent.SetDestination(targetPos);

    //    // Calculate the new position using SmoothDamp logic
    //    Vector3 smoothDampedPosition = Vector3.SmoothDamp(transform.position, agent.nextPosition, ref velocity, 0.1f);

    //    // Calculate the direction and distance to move
    //    Vector3 moveDelta = smoothDampedPosition - transform.position;

    //    controller.Move(transform.position + moveDelta);
    //}

    // Moving around the target via AIManager, circling the target
    public void MoveAIUnit(Vector3 targetPos)
    {
        if (agent.enabled)
        {
            if (isAttacking) return;

            agent.avoidancePriority = 50;

            agent.isStopped = false;
            agent.autoRepath = true;

            agent.SetDestination(targetPos);
        }
    }

    void Stop()
    {
        agent.ResetPath();
        agent.isStopped = true;
        agent.avoidancePriority = 2;
    }

    private void GeneratePlayerHealthBar(Transform hpProxy)
    {
        GameObject healthBarGo = Instantiate(healthBarPrefab);
        healthBar = healthBarGo.GetComponent<Healthbar>();
        healthBar.SetHealthBarData(hpProxy, healthPanelRect);
        healthBar.transform.SetParent(healthPanelRect, false);
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

    private float CurrentSpeed()
    {
        // Calculate the displacement vector since the last frame
        Vector3 displacement = transform.position - lastPosition;

        // Calculate the magnitude of the displacement vector
        float distanceMoved = displacement.magnitude;

        // Calculate the speed based on the distance and time
        calculatedSpeed = distanceMoved / Time.deltaTime;

        // Update the last position
        lastPosition = transform.position;

        return calculatedSpeed;
    }
}
