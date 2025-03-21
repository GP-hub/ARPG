using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.AI;


[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(CharacterController))]
public class Enemy : MonoBehaviour
{

    [Space(10)]
    [Header("Miscellaneous")]
    [SerializeField] private float rotationSpeed = 25f;
    private BlendTree attackBlendTree;
    public float RotationSpeed { get => rotationSpeed; }
    [SerializeField] private float currentHealth, maxHealth = 30;
    [Tooltip("Animator issue when speed is below 2")]
    [SerializeField] private float speed;
    [SerializeField] private int xp;

    [SerializeField] private bool isBoss;
    private bool isPhaseTwo = false;
    private bool isPhaseTree = false;

    private float cCDuration;

    [Space(10)]
    [Header("Healthbar")]
    [SerializeField] private GameObject healthBarPrefab;
    [SerializeField] private RectTransform healthPanelRect;
    [SerializeField] private Transform hpBarProxyFollow;

    [Space(10)]
    [Header("Abilities")]
    [SerializeField] private List<AbilityData> abilities;
    private List<AbilityData> offCooldownAbilities;
    private AbilityData currentAbility;
    private Dictionary<AbilityData, float> abilityCooldowns = new Dictionary<AbilityData, float>();
    private float abilityCheckInterval = 0.5f; 
    private float nextAbilityCheckTime = 0f;


    [Space(10)]
    [Header("Attack")]
    //[SerializeField] private float attackRange;
    [SerializeField] private GameObject exitPoint;
    //[SerializeField] private float attackCooldown;
    //private float currentAttackCooldown;
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
    private bool isAlive = true;
    private bool isCC;

    private bool isLookingForTarget;

    [HideInInspector] public bool isCharging;
    [HideInInspector] public bool isPowerOnCooldown;
    //[HideInInspector] public bool isAttackOnCooldown;

    public IState currentState;

    public Transform Target { get => target; set => target = value; }
    public NavMeshAgent Agent { get => agent; }
    public Animator Animator { get => animator; }
    public bool IsPowering { get => isPowering; }
    public bool IsAttacking { get => isAttacking; }
    public float CCDuration { get => cCDuration; }
    public bool IsCC { get => isCC; set => isCC = value; }


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
        currentHealth = maxHealth;
        GeneratePlayerHealthBar(hpBarProxyFollow);

        player = GameObject.Find("Player");

        if (player)
        {
            // Pass the player as the target for now
            target = player.transform;
        }

        lastPosition = transform.position;

        offCooldownAbilities = new List<AbilityData>(abilities);

        ChangeState(new IdleState());

        StartCoroutine(CheckGroundedStatus());

        GetAnimatorController();
    }


    private void Update()
    {
        currentState.Update();

        HandleStateMachine();

        UpdateSpellCooldowns();
    }

    private void UseAbility(AbilityData ability)
    {
        if (ability.cooldown <= 0) return;
        isAttacking = true;
        offCooldownAbilities.Remove(ability);
        abilityCooldowns[ability] = ability.cooldown;
        StartCoroutine(AbilityCooldownCoroutine(ability));
    }

    private IEnumerator AbilityCooldownCoroutine(AbilityData ability)
    {
        while (abilityCooldowns[ability] > 0)
        {
            abilityCooldowns[ability] -= Time.deltaTime;
            yield return null;
        }

        offCooldownAbilities.Add(ability);
    }

    private void GetAnimatorController()
    {
        // Get the AnimatorController
        AnimatorController controller = animator.runtimeAnimatorController as AnimatorController;
        if (controller == null) return;

        foreach (AnimatorControllerLayer layer in controller.layers)
        {
            foreach (ChildAnimatorState state in layer.stateMachine.states)
            {
                if (state.state.motion is BlendTree blendTree)
                {
                    attackBlendTree = blendTree;
                    PopulateBlendTree(attackBlendTree);
                    return;
                }
            }
        }
    }

    private void PopulateBlendTree(BlendTree blendTree)
    {
        if (abilities == null || abilities.Count == 0) return;

        //blendTree.useAutomaticThresholds = false; // Set manual thresholds if needed

        ChildMotion[] newChildren = new ChildMotion[abilities.Count];

        for (int i = 0; i < abilities.Count; i++)
        {
            newChildren[i] = new ChildMotion
            {
                motion = abilities[i].animationClip,
                threshold = i,
                timeScale = 1f // Set the speed of the animation to 1
            };
        }

        blendTree.children = newChildren;
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
                    //Debug.Log("Collided with a character: " + collider.name);

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
        currentHealth -= damageAmount;

        if (currentHealth <= 0)
        {
            // Do the correct logic to get rid of dead enemies here
            Death();
            //Destroy(gameObject);
        }
        healthBar.OnHealthChanged(currentHealth / maxHealth);
        //Debug.Log("Enemy hp: " + health);
    }

    private void Death()
    {
        this.gameObject.tag = "Dead";
        isAlive = false;
        Debug.Log("Enemy is dead.");
        animator.SetTrigger("TriggerDeath");
        EventManager.EnemyDeath(xp);
    }

    private void TriggerAnimationOnDeath()
    {
        controller.enabled = false;
        agent.enabled = false;
    }

    void HandleStateMachine()
    {
        if (!isAlive)
        {
            ChangeState(new DeathState());
            return;
        }

        // Update CC duration
        if (cCDuration > 0)
        {
            cCDuration -= Time.deltaTime;
        }

        if (IsCC) return;

        if (target == null || target.tag == "Dead")
        {
            ChangeState(new IdleState());
            if (!isLookingForTarget)
            {
                StartCoroutine(SearchForTargetRoutine());
            }
            return;
        }

        if (CanSeeTarget(target) && target.tag != "Dead")
        {
            // Previous method of calculating distance that do one more operation: a square root
            //float distanceToTarget1 = Vector3.Distance(transform.position, target.position);
            float distanceToTarget = (transform.position - target.position).sqrMagnitude;

            if (isPowering || isAttacking) return;

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
                AbilityFilteringAndSorting(distanceToTarget);

                bool canAttack = offCooldownAbilities.Count > 0;

                if (canAttack)
                {
                    ChangeState(new AttackState());
                    return;
                }
                else
                {
                    if (distanceToTarget <= 1)
                    {
                        ChangeState(new IdleState());
                        return;
                    }
                    else
                    {
                        ChangeState(new FollowState());
                        return;
                    }
                }
            }
        }
        if (!CanSeeTarget(target) && target.tag != "Dead")
        {
            ChangeState(new FollowState());
            return;
        }
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

    private void AbilityFilteringAndSorting(float distanceToTarget)
    {
        if (Time.time < nextAbilityCheckTime) return;
        nextAbilityCheckTime = Time.time + abilityCheckInterval;

        //Debug.Log("Abilities list: " + offCooldownAbilities.Count + " distance to target: " + distanceToTarget);

        // Filter out abilities where the distance to the target is not between minAttackRange and maxAttackRange
        for (int i = offCooldownAbilities.Count - 1; i >= 0; i--)
        {
            AbilityData obj = offCooldownAbilities[i];
            if (distanceToTarget < obj.minAttackRange || distanceToTarget > obj.maxAttackRange)
            {
                offCooldownAbilities.RemoveAt(i);
            }
        }
        // Add abilities that are now in range and not on cooldown
        foreach (AbilityData ability in abilities)
        {
            if (distanceToTarget >= ability.minAttackRange && distanceToTarget <= ability.maxAttackRange && !offCooldownAbilities.Contains(ability))
            {
                if (!abilityCooldowns.ContainsKey(ability) || abilityCooldowns[ability] <= 0)
                {
                    offCooldownAbilities.Add(ability);
                }
            }
        }

        offCooldownAbilities.Sort((a, b) => b.cooldown.CompareTo(a.cooldown));
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
                EventManager.PlayerTakeDamage(currentAbility.damage);
                Debug.Log("dmg: " + currentAbility.damage);
            }
        }
        //ResetAttackingAndPowering();
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
                fireball.Damage = currentAbility.damage;
            }
            Quaternion rotationToTarget = Quaternion.LookRotation(direction);
            newObject.transform.rotation = rotationToTarget;
        }
        //ResetAttackingAndPowering();
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



    public IEnumerator SearchForTargetRoutine()
    {
        isLookingForTarget = true;

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
                        isLookingForTarget = false;
                        yield return null;
                        //Debug.Log("Can see player in aggro range.");
                    }
                    if (!CanSeeTarget(t))
                    {
                        //Debug.Log("Target in range but not in sight");
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

    public void SetBoolSingle(string triggerName)
    {
        foreach (AnimatorControllerParameter param in animator.parameters)
        {
            if (param.type == AnimatorControllerParameterType.Bool)
            {
                animator.SetBool(param.name, false);
            }
        }

        // Enable the desired trigger
        animator.SetBool(triggerName, true);
    }

    public void ResetAllAnimatorTriggers()
    {
        foreach (AnimatorControllerParameter param in animator.parameters)
        {
            if (param.type == AnimatorControllerParameterType.Bool)
            {
                animator.SetBool(param.name, false);
            }
        }
    }
    public void ResetSingleBool(string triggerName)
    {
        animator.SetBool(triggerName, false);
    }

    public void CastAttack()
    {
        //currentAttackCooldown = attackCooldown;
        //isAttackOnCooldown = true;
        //isAttacking = true;
    }

    public void CastPower()
    {
        currentPowerCooldown = powerCooldown;
        isPowerOnCooldown = true;
        isPowering = true;
    }

    private void UpdateSpellCooldowns()
    {

        //if (currentAttackCooldown > 0f && isAttackOnCooldown)
        //{
        //    currentAttackCooldown -= Time.deltaTime;
        //}
        //if (currentAttackCooldown <= 0 && isAttackOnCooldown)
        //{
        //    isAttackOnCooldown = false;
        //}

        if (currentPowerCooldown > 0f && isPowerOnCooldown)
        {
            currentPowerCooldown -= Time.deltaTime;
        }
        if (currentPowerCooldown <= 0 && isPowerOnCooldown)
        {
            isPowerOnCooldown = false;
        }
    }

    public void UpdateCCDuration(float newCCDuration)
    {
        //Debug.Log("cCDuration:" + cCDuration + ", newCCDuration:" + newCCDuration);
        if (newCCDuration >= cCDuration)
        {
            cCDuration = newCCDuration;
        }
    }

    public void ResetAttackingAndPowering()
    {
        if (isAttacking) isAttacking = false;
        if (isPowering) isPowering = false;
    }


    /// ///////////////////////////////////////////////////////////////////////////////

    public void DecideNextAbility()
    {
        if (isBoss) DecideNextBossMoveID();
        else DecideNextMoveID();
    }

    // 1 = basic attack, 2 = charge attack, 3 = jump attack, 4 = ranged attack
    private float[] possibleValues = { 0f, 0.2f, 0.5f, 1f };

    private void DecideNextMoveID()
    {
        // Get a random index based on the length of the array
        //int randomIndex = UnityEngine.Random.Range(0, possibleValues.Length);
        //Debug.Log("Attack: " + possibleValues[randomIndex]);

        //if (currentHealth <= 0.5f * maxHealth)
        //{
        //    Debug.Log("below 50% hp");
        //    currentAbility = abilities[0];
        //}
        //else
        //{
        //    Debug.Log("higher than 50% hp");
        //    currentAbility = abilities[1];
        //}
        currentAbility = offCooldownAbilities[0];
        UseAbility(currentAbility);
        Debug.Log("Cast ability:" + currentAbility.name);
    }

    private void DecideNextBossMoveID()
    {
        // If we are not already in phase 2 => WE ENTER PHASE 2 and we play ONCE the phase 2 ability 
        if (currentHealth <= 0.75f * maxHealth && !isPhaseTwo)
        {
            Debug.Log("ENTER PHASE TWO");
            isPhaseTwo = true;
        }
        // If we are not already in phase 3 => WE ENTER PHASE 3 and we play ONCE the phase 3 ability 
        if (currentHealth <= 0.50f * maxHealth && !isPhaseTree)
        {
            Debug.Log("ENTER PHASE THREE");
            isPhaseTree = true;
        }
        // otherwise basic attack
        else
        {
            currentAbility = offCooldownAbilities[0];
        }

        UseAbility(currentAbility);
    }

    public float BlendTreeThreshold()
    {
        return Utility.GetClipThreshold(attackBlendTree, currentAbility.animationClip);
    }

    // Useless but present in some animation so keep it to avoid null refs
    public void DecideNextMove() { }

    public void BossRockFall()
    {
        Debug.Log("ROCKS ARE FALLING HERE");
    }


    public void JumpAttack()
    {
        if (target == null) return;

        // distance from the target
        float jumpDistance = 1f;

        Vector3 directionToTarget = (target.position - transform.position).normalized;
        Vector3 finalPosition = target.position - directionToTarget * jumpDistance;

        StartCoroutine(JumpToLocation(finalPosition));
    }


    private IEnumerator JumpToLocation(Vector3 destination)
    {
        float jumpDuration = 1f; // Adjust the duration as needed
        float t = 0f;
        Vector3 startPosition = transform.position;
        isCharging = true;

        while (t < jumpDuration)
        {
            transform.position = Vector3.Lerp(startPosition, destination, (t / jumpDuration));
            t += Time.deltaTime;
            yield return null;
        }

        transform.position = destination;

        // wait time before the next action, so we dont rotate towards target while ending the animation
        yield return new WaitForSeconds(1.5f);
        isCharging = false;
    }


}
