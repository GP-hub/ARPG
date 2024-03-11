using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;


[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(CharacterController))]
public class Enemy : MonoBehaviour
{
    [SerializeField] private float health, maxHealth = 30;
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
    [SerializeField] private float attackRange;
    [SerializeField] private int attackDamage;
    [SerializeField] private GameObject exitPoint;
    [SerializeField] private float attackCooldown;
    private float currentAttackCooldown;
    [SerializeField] private LayerMask characterLayer;
    private float lastAttackTime;

    [Space(10)]
    [Header("Attack: Ranged")]
    [SerializeField] private string fireballPrefabName;
    [SerializeField] private float attackProjectileSpeed;

    [Space(10)]
    [Header("Attack: Melee")]
    [SerializeField] private float meleeHitboxSize;

    [Space(10)]
    [Header("Power")]
    [SerializeField] private bool hasPowerAbility;
    [SerializeField] private string AoePrefabName;
    [SerializeField] private int powerDamage;
    [SerializeField] private float powerRange;
    [SerializeField] private float powerCooldown;
    private float currentPowerCooldown;
    private float lastPowerTime;


    private NavMeshAgent agent;

    private Animator animator;

    private CharacterController controller;

    private Healthbar healthBar;

    private Transform target;

    private GameObject player;

    private Vector3 lastPosition;

    private float calculatedSpeed;

    private bool isGrounded = true;
    private bool isAttacking;
    private bool isPowering = false;
    private bool isMoving;
    private bool isIdle;    

    [HideInInspector] public bool isCharging;
    [HideInInspector] public bool isPowerOnCooldown;
    [HideInInspector] public bool isAttackOnCooldown;

    public IState currentState;

    public Transform Target { get => target; set => target = value; }
    public NavMeshAgent Agent { get => agent;}
    public Animator Animator { get => animator;}
    public bool IsPowering { get => isPowering;}
    public bool IsAttacking { get => isAttacking;}

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

        player = GameObject.Find("Player");

        if (player)
        {
            // Pass the player as the target for now
            target = player.transform;
        }

        lastPosition = transform.position;

        ChangeState(new IdleState());

        StartCoroutine(CheckGroundedStatus());
    }


    private void Update()
    {
        currentState.Update();

        HandleStateMachine();

        UpdateSpellCooldowns();
    }

    private void ChargingCoroutineStart()
    {
        StartCoroutine(MoveForwardCoroutine());
    }

    IEnumerator MoveForwardCoroutine()
    {
        float timer = .5f;
        isCharging = true;
        while (timer > 0f)
        {
            //this.transform.LookAt(this.transform.forward);
            // Move the CharacterController forward
            controller.Move(transform.forward * speed * 5f * Time.deltaTime);

            agent.avoidancePriority = 10;
            agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;

            // Check for collisions with objects on the 'Player' layer
            Collider[] colliders = Physics.OverlapBox(transform.position, transform.localScale / 2, Quaternion.identity, LayerMask.GetMask("Character"));

            foreach (Collider collider in colliders)
            {
                if (collider.CompareTag("Player"))
                {
                    Debug.Log("Collided with a character: " + collider.name);

                    // Handle collision with 'Player' here
                }
            }

            // Decrease the timer
            timer -= Time.deltaTime;

            // Wait for the next frame
            yield return null;
        }
        isCharging = false;
        ResetAttackingAndPowering();

        agent.obstacleAvoidanceType = ObstacleAvoidanceType.LowQualityObstacleAvoidance;

        agent.avoidancePriority = 50;

        // Stop the CharacterController when the time is up
        controller.Move(Vector3.zero);
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

    void HandleStateMachine()
    {
        if (target == null)
        {
            StartCoroutine(SphereCastRoutine());
            return;
        }

        if (CanSeeTarget(target))
        {
            // Previous method of calculating distance that do one more operation: a square root, not sure what is the difference with the one below.
            //float distanceToTarget1 = Vector3.Distance(transform.position, target.position);
            float distanceToTarget = (transform.position - target.position).sqrMagnitude;

            if (isPowering || IsAttacking) return;

            if (!isPowerOnCooldown && hasPowerAbility)
            {
                if (distanceToTarget <= powerRange)
                {
                    ChangeState(new PowerState());
                    return;
                }
                else if (distanceToTarget > powerRange)
                {
                    ChangeState(new FollowState());
                    return;
                }
            }
            else if (isPowerOnCooldown || !hasPowerAbility)
            {
                if (distanceToTarget <= attackRange)
                {
                    ChangeState(new AttackState());
                    return;
                }
                else if (distanceToTarget > attackRange)
                {
                    ChangeState(new FollowState());
                    return;
                }
            }
        }
        if (!CanSeeTarget(target))
        {
            ChangeState(new FollowState());
            return;
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
                EventManager.Instance.PlayerTakeDamage(attackDamage);
            }
        }
        ResetAttackingAndPowering();
    }

    // Triggered via SpawnAoe Attack Animation
    public void SpawnAOE()
    {
        if (!target) return;
        GameObject newObject = PoolingManagerSingleton.Instance.GetObjectFromPool(AoePrefabName, target.transform.position + new Vector3(0, 0.2f, 0));

        if (newObject.TryGetComponent<AbilityValues>(out AbilityValues aoeSpell))
        {
            aoeSpell.Damage = powerDamage;
            aoeSpell.DoDamage(powerDamage);
        }
        ResetAttackingAndPowering();
    }



    // Triggered via Ranged Attack animation
    public void RangedHit()
    {
        if (!target) return;

        Vector3 targetCorrectedPosition = target.transform.position;
        Vector3 direction = (targetCorrectedPosition - this.transform.position).normalized;

        GameObject newObject = PoolingManagerSingleton.Instance.GetObjectFromPool(fireballPrefabName, exitPoint.transform.position);

        if (newObject != null)
        {
            if (newObject.TryGetComponent<AbilityValues>(out AbilityValues fireball))
            {
                fireball.Damage = attackDamage;
            }
            Quaternion rotationToTarget = Quaternion.LookRotation(direction);
            newObject.transform.rotation = rotationToTarget;
        }
        ResetAttackingAndPowering();
    }

    // Moving around the target via AIManager, circling the target
    public void MoveAIUnit(Vector3 targetPos)
    {
        if (agent.enabled)
        {
            if (isAttacking) return;

            agent.avoidancePriority = 50;

            agent.isStopped = false;

            agent.SetDestination(targetPos);
        }
    }

    public void Stop()
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

    private bool CanSeeTarget(Transform target)
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

    public void ChangeState(IState newState)
    {

        if (currentState != null)
        {
            if (newState.GetStateName() == currentState.GetStateName()) return;

            currentState.Exit();
        }

        currentState = newState;
        currentState.Enter(this);
    }

    public IEnumerator SphereCastRoutine()
    {
        while (target == null) 
        {
            // Max number of entities in the OverlapSphere
            int maxColliders = 10;
            Collider[] hitColliders = new Collider[maxColliders];
            int numColliders = Physics.OverlapSphereNonAlloc(transform.position, 50f, hitColliders, characterLayer);

            for (int i = 0; i < numColliders; i++)
            {
                if (hitColliders[i].CompareTag("Player"))
                {
                    Transform t = hitColliders[i].transform;
                    if (CanSeeTarget(t))
                    {
                        target = t;
                        //Debug.Log("Can see player in aggro range.");
                    }
                    if (!CanSeeTarget(t))
                    {
                        //Debug.Log("Player in range but cannot see.");
                    }
                }
            }
            if (hitColliders.Length <= 0)
            {
                // No character hit
                Debug.Log("No character hit.");
            }

            // Wait for 2 seconds before performing the next SphereCast
            yield return new WaitForSeconds(2f);
        }
    }

    public void SetTriggerSingle(string triggerName)
    {
        // Disable all triggers
        foreach (AnimatorControllerParameter param in animator.parameters)
        {
            if (param.type == AnimatorControllerParameterType.Trigger)
            {
                animator.ResetTrigger(param.name);
            }
        }

        // Enable the desired trigger
        animator.SetTrigger(triggerName);
    }

    public void ResetAllAnimatorTriggers()
    {
        // Disable all triggers
        foreach (AnimatorControllerParameter param in animator.parameters)
        {
            if (param.type == AnimatorControllerParameterType.Trigger)
            {
                animator.ResetTrigger(param.name);
            }
        }
    }
    public void ResetTriggerSingle(string triggerName)
    {
        animator.ResetTrigger(triggerName);
    }

    public void CastAttack()
    {
        currentAttackCooldown = attackCooldown;
        isAttackOnCooldown = true;
        isAttacking = true;
    }

    public void CastPower()
    {
        currentPowerCooldown = powerCooldown;
        isPowerOnCooldown = true;
        isPowering = true;
    }

    private void UpdateSpellCooldowns()
    {

        if (currentAttackCooldown > 0f && isAttackOnCooldown)
        {
            currentAttackCooldown -= Time.deltaTime;
        }
        if (currentAttackCooldown <= 0 && isAttackOnCooldown)
        {
            isAttackOnCooldown = false;
        }

        if (currentPowerCooldown > 0f && isPowerOnCooldown)
        {
            currentPowerCooldown -= Time.deltaTime;
        }
        if (currentPowerCooldown <= 0 && isPowerOnCooldown)
        {
            isPowerOnCooldown = false;
        }
    }

    // Triggered last frame of every enemy skill animations
    private void DecideNextMove()
    {

    }

    public void ResetAttackingAndPowering()
    {
        if (IsAttacking) isAttacking = false;
        if (isPowering) isPowering = false;
    }
}
