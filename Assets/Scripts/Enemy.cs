using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;


[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(CharacterController))]
public class Enemy : MonoBehaviour
{

    [Space(10)]
    [Header("Miscellaneous")]
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float moveStateSpeed;
    [SerializeField] private float attackStateSpeed;

    [SerializeField] private RuntimeAnimatorController baseController;
    private AnimatorOverrideController runtimeOverride;

    // Phase handling
    private int currentPhase = 1;
    private int previousPhase = 1;
    public bool hasPhaseJustChanged;

    public float RotationSpeed { get => rotationSpeed; }
    [SerializeField] private float currentHealth, maxHealth;
    [Tooltip("Animator issue when speed is below 2")]
    [SerializeField] private float speed;
    [SerializeField] private int xp;
    [SerializeField] private int searchTargetRadius;

    [SerializeField] private bool isBoss;

    private float cCDuration;

    [Space(10)]
    [Header("Healthbar")]
    [SerializeField] private GameObject healthBarPrefab;
    [SerializeField] private RectTransform healthPanelRect;
    [SerializeField] private Transform hpBarProxyFollow;

    [Space(10)]
    [Header("Abilities")]
    [SerializeField] private float accuracyPercent = 100;
    [SerializeField] private List<AbilityData> abilities;
    private List<AbilityData> offCooldownAbilities;
    private AbilityData currentAbility;
    private float minMaxAbilityRange;
    private Dictionary<AbilityData, float> abilityCooldowns = new Dictionary<AbilityData, float>();
    private float nextCastTime = 0f;
    private float delayBetweenAbilities = 0.5f;

    public bool CanCastAbility => Time.time >= nextCastTime;

    [Space(10)]
    [Header("Power Abilities")]
    [SerializeField] private AbilityData powerAbility;
    private float currentPowerCooldown;


    [Space(10)]
    [Header("Attack")]
    [SerializeField] private GameObject exitPoint;
    [SerializeField] private LayerMask characterLayer;

    [Space(10)]
    [Header("Attack: Melee")]
    [SerializeField] private float meleeHitboxSize;


    private NavMeshAgent agent;

    private Animator animator;

    private CharacterController controller;

    private Healthbar healthBar;

    private Transform target;

    private Vector3 targetPosition;

    private GameObject player;

    private Vector3 lastPosition;

    private float calculatedSpeed;

    private bool isGrounded = true;
    private bool isAttacking;
    private bool isPowering = false;
    public bool isMoving;
    private bool isIdle;
    private bool isAlive = true;
    private bool isCC;


    private bool isLookingForTarget;

    [HideInInspector] public bool isCharging;
    [HideInInspector] public bool isPowerOnCooldown;

    public IState currentState;

    public Transform Target { get => target; set => target = value; }
    public Vector3 TargetPosition { get => targetPosition; set => targetPosition = value; }
    public NavMeshAgent Agent { get => agent; }
    public Animator Animator { get => animator; }
    public bool IsPowering { get => isPowering; }
    public bool IsAttacking { get => isAttacking; }
    public float CCDuration { get => cCDuration; set => cCDuration = value; }
    public bool IsCC { get => isCC; set => isCC = value; }
    public float CurrentHealth { get => currentHealth; }
    public float MaxHealth { get => maxHealth; }
    public bool IsBoss { get => isBoss; }
    public int CurrentPhase { get => currentPhase; }

    private void Awake()
    {
        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        agent = GetComponent<NavMeshAgent>();
        GetAnimatorOverrideController();
    }

    // 
    void Start()
    {
        agent.speed = speed;
        
        GenerateEnemyHealthBar(hpBarProxyFollow);

        player = GameObject.Find("Player");

        offCooldownAbilities = new List<AbilityData>();

        StartCoroutine(CheckGroundedStatus());
    }


    private void Update()
    {
        currentState.Update();

        HandleStateMachine();

        UpdateSpellCooldowns();

    }


    private void OnEnable()
    {
        EventManager.onGetUnits += AddEnemyToAIManager;
        this.animator.SetFloat("MoveStateSpeed", moveStateSpeed); // Reset the attack tree to 0 on enable
        this.animator.SetFloat("AttackStateSpeed", attackStateSpeed); // Reset the attack tree to 0 on enable
        currentHealth = maxHealth;
        lastPosition = transform.position;
        minMaxAbilityRange = MinMaxRangeAttackRange();
        StartCastCooldown();
        ChangeState(new IdleState());
    }

    private void OnDisable()
    {
        EventManager.onGetUnits -= AddEnemyToAIManager;
    }

    private void AddEnemyToAIManager()
    {
        AIManager.Instance.AddUnit(this);
    }

    public void StartCastCooldown()
    {
        nextCastTime = Time.time + delayBetweenAbilities;
    }

    private void UseAbility(AbilityData ability)
    {
        //if (ability.cooldown <= 0) return;
        isAttacking = true;
        offCooldownAbilities.Remove(ability);
        abilityCooldowns[ability] = ability.cooldown;
    }

    public void TryUseFollowUpAbility()
    {
        if (currentAbility == null || currentAbility.followUpAbility == null)
            return;

        AbilityData followUp = currentAbility.followUpAbility;

        int followUpIndex = abilities.IndexOf(followUp);
        if (followUpIndex != -1)
        {
            ChangeState(new AttackState(followUpIndex));
        }
        else
        {
            Debug.LogWarning($"Follow-up ability '{followUp.name}' not found in enemy abilities list.");
        }
    }

    private void UseAbilityNoCooldown(AbilityData ability)
    {
        isAttacking = true;
    }

    private void UsePowerAbility(AbilityData ability)
    {
        //if (ability.cooldown <= 0) return;
        isPowering = true;
        isPowerOnCooldown = true;
        currentPowerCooldown = ability.cooldown;
    }

    private void GetAnimatorOverrideController()
    {
        runtimeOverride = new AnimatorOverrideController(baseController);
        animator.runtimeAnimatorController = runtimeOverride;

        for (int i = 0; i < abilities.Count; i++)
        {
            string dummyClipName = $"Attack_{i}";
            AnimationClip realClip = abilities[i].animationClip;

            if (realClip != null)
            {
                runtimeOverride[dummyClipName] = realClip;
            }
            else
            {
                Debug.LogWarning($"Real clip missing for ability index {i}");
            }
        }

        if (powerAbility!= null)
        {
            string dummyClipName = "Power_0";
            AnimationClip realClip = powerAbility.animationClip;

            if (realClip != null)
            {
                runtimeOverride[dummyClipName] = realClip;
            }
            else
            {
                Debug.LogWarning($"Real clip missing for power ability index");
            }
        }
    }


    private IEnumerator CheckGroundedStatus()
    {
        while (true)
        {
            isGrounded = controller.isGrounded;

            if (isGrounded) agent.enabled = true;
            else agent.enabled = false;

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
        UpdateCurrentPhase();

        DamageNumberPool.Instance.ShowDamage(this.transform.position + Vector3.up * (this.transform.localScale.y + 0.85f), Mathf.CeilToInt(damageAmount).ToString(), Color.white);

        //Debug.Log("Enemy hp: " + health);
    }

    private void UpdateCurrentPhase()
    {
        int oldPhase = currentPhase;

        if (currentHealth <= 0.66f * maxHealth && currentPhase == 1)
        {
            currentPhase = 2;
        }
        else if (currentHealth <= 0.33f * maxHealth && currentPhase == 2)
        {
            currentPhase = 3;
        }

        if (oldPhase != currentPhase)
        {
            hasPhaseJustChanged = true;
        }
    }

    private void ConsumePhaseChanged()
    {
        hasPhaseJustChanged = false;  // auto-clear
    }


    private void Death()
    {
        this.gameObject.tag = "Dead";
        isAlive = false;
        Debug.Log(this.name + " is dead.");
        animator.SetTrigger("TriggerDeath");
        EventManager.EnemyDeath(xp);

        if (isBoss) EventManager.BossDeath();

        StopAllCoroutines();
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

        // Update CC duration, we cant stun bosses
        if (cCDuration > 0)
        {
            ChangeState(new StunState());
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


        if (/*CanSeeTarget(target) && */target.tag != "Dead")
        {
            if (isMoving)
            {
                return;
            }

            if (!CanCastAbility)
            {
                ChangeState(new IdleState());
                return;
            }
            // Previous method of calculating distance that do one more operation: a square root
            //float distanceToTarget1 = Vector3.Distance(transform.position, target.position);
            float distanceToTarget = (transform.position - target.position).sqrMagnitude;

            if (isPowering || isAttacking) return;
            if (CanPower(distanceToTarget) && AreConditionsMet(powerAbility))
            {

                ChangeState(new PowerState());
                return;

            }
            else if (!CanPower(distanceToTarget))
            {
                AbilityFilteringAndSorting(distanceToTarget);

                bool abilityAvailable = offCooldownAbilities.Count > 0;
                //Debug.Log("distance to target:" + distanceToTarget);

                if (abilityAvailable)
                {
                    ChangeState(new AttackState());
                    return;
                }
                else
                {
                    if (distanceToTarget < minMaxAbilityRange && CanSeeTarget(target))
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

        if (isPowering || isAttacking || isCharging) return;

        if (!CanSeeTarget(target) && target.tag != "Dead")
        {
            Debug.Log("Target not in sight, FOLLOW");
            ChangeState(new FollowState());
            return;
        }
    }

    private bool CanPower(float distanceToTarget)
    {
        if (powerAbility == null) return false;
        if (isPowerOnCooldown) return false;
        if (!AreConditionsMet(powerAbility)) return false;

        if (currentPowerCooldown <= 0 && distanceToTarget < powerAbility.maxAttackRange && distanceToTarget > powerAbility.minAttackRange)
        {
            return true;
        }
        return false;
    }

    private float MinMaxRangeAttackRange()
    {
        if (abilities.Count == 0)
        {
            return 1.1f; // Default value if no abilities are available
        }

        float minMaxAttackRange = float.MaxValue;

        foreach (AbilityData ability in abilities)
        {
            if (ability.maxAttackRange < minMaxAttackRange)
            {
                minMaxAttackRange = ability.maxAttackRange;
            }
        }

        if (minMaxAttackRange <= 0) Debug.Log($"Issue with setting up ability range on {gameObject.name}. minMaxAttackRange is {minMaxAttackRange}");

        return minMaxAttackRange;
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

        //Add abilities that are now in range and not on cooldown
        foreach (AbilityData ability in abilities)
        {
            if (distanceToTarget >= ability.minAttackRange && distanceToTarget <= ability.maxAttackRange && !offCooldownAbilities.Contains(ability))
            {
                if (!abilityCooldowns.ContainsKey(ability) || abilityCooldowns[ability] <= 0)
                {
                    if (AreConditionsMet(ability))
                    {
                        offCooldownAbilities.Add(ability);
                    }
                }
            }
        }



        // Filter out abilities where the distance to the target is not between minAttackRange and maxAttackRange
        for (int i = offCooldownAbilities.Count - 1; i >= 0; i--)
        {
            AbilityData ability = offCooldownAbilities[i];
            if (distanceToTarget < ability.minAttackRange || distanceToTarget > ability.maxAttackRange)
            {
                offCooldownAbilities.RemoveAt(i);
                //offCooldownAbilities.Remove(ability);
            }
        }

        offCooldownAbilities.Sort((a, b) => b.cooldown.CompareTo(a.cooldown));
        //Debug.Log("Off cooldown abilities: " + offCooldownAbilities.Count + ", distance to target: " + distanceToTarget);
    }

    private bool AreConditionsMet(AbilityData ability)
    {
        if (ability.conditions != null && ability.conditions.Count > 0)
        {
            foreach (ScriptableAbilityCondition condition in ability.conditions)
            {
                bool isConditionMet = condition.IsMet(this);

                if (!isConditionMet)
                    return false;
            }
            return true; // All conditions passed
        }
        else
        {
            return true; // No conditions = always valid
        }

    }


    public void PerformAttack()
    {

        if (currentAbility == null || string.IsNullOrEmpty(currentAbility.selectedFunctionName))
        {
            Debug.Log($"{gameObject.name} has no ability or function selected.");
            return;
        }

        // Get the method by name
        MethodInfo method = GetType().GetMethod(currentAbility.selectedFunctionName,
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        if (method != null)
        {
            method.Invoke(this, null);
        }
        else
        {
            Debug.Log($"Method '{currentAbility.selectedFunctionName}' not found on {gameObject.name}");
        }
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

    private void GenerateEnemyHealthBar(Transform hpProxy)
    {
        GameObject healthBarGo = Instantiate(healthBarPrefab);
        healthBar = healthBarGo.GetComponent<Healthbar>();
        healthBar.SetHealthBarData(hpProxy, healthPanelRect);
        healthBar.transform.SetParent(healthPanelRect, false);
    }

    public bool CanSeeTarget(Transform target)
    {
        Vector3 capsuleStart = transform.position;
        Vector3 capsuleEnd = exitPoint.transform.position;
        Vector3 targetPos = target.position + new Vector3(0f, 1f, 0f);

        Vector3 direction = (targetPos - capsuleStart).normalized;
        float radius = 0.2f;

        RaycastHit[] hits = Physics.CapsuleCastAll(
            capsuleStart,
            capsuleEnd,
            radius,
            direction,
            300,
            LayerMask.GetMask("Character", "Obstacle")
        );

        float closestHitDist = Mathf.Infinity;
        RaycastHit? closestHit = null;

        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.gameObject == gameObject) continue; // ignore self

            if (hit.distance < closestHitDist)
            {
                closestHit = hit;
                closestHitDist = hit.distance;
            }
        }

        return closestHit.HasValue && closestHit.Value.collider.CompareTag("Player");
    }

    public Vector3 GetInaccurateTarget(Vector3 originalTarget)
    {

        // Random point in a circle (2D on XZ plane)
        Vector2 randomOffset = Random.insideUnitCircle * (currentAbility.accuracyRange * (1 - accuracyPercent / 100f));

        // Add the offset to the original target
        Vector3 inaccurateTarget = new Vector3(
            originalTarget.x + randomOffset.x,
            originalTarget.y,
            originalTarget.z + randomOffset.y
        );

        return inaccurateTarget;
    }



    public IEnumerator SearchForTargetRoutine()
    {
        isLookingForTarget = true;

        while (target == null)
        {
            // Max number of entities in the OverlapSphere
            int maxColliders = 10;
            Collider[] hitColliders = new Collider[maxColliders];
            int numColliders = Physics.OverlapSphereNonAlloc(transform.position, searchTargetRadius, hitColliders, characterLayer);

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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, searchTargetRadius);
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



    private void UpdateSpellCooldowns()
    {
        List<AbilityData> abilitiesToReset = new List<AbilityData>();

        foreach (AbilityData ability in new List<AbilityData>(abilityCooldowns.Keys))
        {
            if (abilityCooldowns[ability] > 0)
            {
                abilityCooldowns[ability] -= Time.deltaTime;
            }
            if (abilityCooldowns[ability] <= 0 && !offCooldownAbilities.Contains(ability))
            {
                abilitiesToReset.Add(ability);
            }
        }

        // Reset abilities after iteration
        foreach (AbilityData ability in abilitiesToReset)
        {
            abilityCooldowns[ability] = 0;
            if (AreConditionsMet(ability))
            {
                offCooldownAbilities.Add(ability);
            }
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

    public void UpdateCCDuration(float newCCDuration, string source)
    {
        if (isBoss && source == "Player") return; // bosses cant get CCed by players

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

    public void ForceAbility(int index)
    {
        if (index >= 0 && index < abilities.Count)
        {
            currentAbility = abilities[index];
            UseAbilityNoCooldown(currentAbility);
        }
        else
        {
            Debug.LogWarning("Invalid ability index passed to ForceAbility");
            DecideNextAbility(); // fallback
        }
    }

    public void DecideNextAbility()
    {
        if (isBoss) DecideNextBossMoveID();
        else DecideNextMoveID();
    }

    public void DecideNextPowerAbility()
    {
        DecideNextPowerMoveID();
    }

    private void DecideNextPowerMoveID()
    {
        currentAbility = powerAbility;
        UsePowerAbility(currentAbility);
    }

    private void DecideNextMoveID()
    {
        currentAbility = offCooldownAbilities[0];
        UseAbility(currentAbility);
    }

    private void DecideNextBossMoveID()
    {
        currentAbility = offCooldownAbilities[0];

        UseAbility(currentAbility);
    }
    public AbilityData GetCurrentAbility()
    {
        return currentAbility;
    }


    public float GetCurrentAbilityIndex()
    {
        return abilities.IndexOf(currentAbility);
    }
    public float GetCurrentPowerAbilityIndex()
    {
        // Because right now we have only one power ability, we return 0
        return 0;
    }

    [AttackMethod]
    public void MoveToPosition()
    {
        ChangeState(new MoveToState(new Vector3(206, 0, -28)));
        //ConsumePhaseChanged();
    }


    [AttackMethod]
    public void JumpAttack()
    {
        if (target == null) return;

        //float jumpDistance = 1f;

        //Vector3 directionToTarget = (TargetPosition - transform.position).normalized;
        //Vector3 finalPosition = TargetPosition - directionToTarget * jumpDistance;

        StartCoroutine(JumpToLocation(/*finalPosition*/));
    }

    private IEnumerator JumpToLocation(/*Vector3 destination*/)
    {
        //TEST
        TargetPosition = GetInaccurateTarget(Target.position);

        float jumpDuration = (currentAbility.animationClip.length / attackStateSpeed) * 0.5f; // Adjust the duration as needed

        float t = 0f;
        Vector3 startPosition = transform.position;
        isCharging = true;

        while (t < jumpDuration)
        {
            transform.position = Vector3.Lerp(startPosition, TargetPosition, (t / jumpDuration));
            t += Time.deltaTime;
            yield return null;
        }

        // Max number of entities in the OverlapSphere
        int maxColliders = 10;
        Collider[] hitColliders = new Collider[maxColliders];
        int numColliders = Physics.OverlapSphereNonAlloc(transform.position, meleeHitboxSize, hitColliders, characterLayer);

        for (int i = 0; i < numColliders; i++)
        {
            if (hitColliders[i].CompareTag("Player"))
            {
                EventManager.PlayerTakeDamage(currentAbility.damage);
            }
        }

        // We check for rocks to destroy them
        int maxRockColliders = 2;
        Collider[] hitRocksColliders = new Collider[maxRockColliders];
        int numRocksColliders = Physics.OverlapSphereNonAlloc(transform.position, meleeHitboxSize + 1, hitRocksColliders, LayerMask.GetMask("Obstacle"));

        for (int i = 0; i < numRocksColliders; i++)
        {
            if (hitRocksColliders[i].CompareTag("Destructible"))
            {
                hitRocksColliders[i].gameObject.SetActive(false);
            }
        }

        //transform.position = TargetPosition;

        // wait time before the next action, so we dont rotate towards target while ending the animation
        yield return new WaitForSeconds(.5f);
        isCharging = false;
    }

    [AttackMethod]
    public void ChargingCoroutineStart()
    {
        StartCoroutine(MoveForwardCoroutine());
    }

    IEnumerator MoveForwardCoroutine()
    {
        float timer = .5f;
        isCharging = true;
        while (timer > 0f)
        {
            controller.Move(transform.forward * speed * 5f * Time.deltaTime);

            agent.avoidancePriority = 10;
            agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;

            Collider[] colliders = Physics.OverlapBox(transform.position, transform.localScale / 2, Quaternion.identity, characterLayer);

            foreach (Collider collider in colliders)
            {
                if (collider.CompareTag("Player"))
                {
                    EventManager.PlayerTakeDamage(currentAbility.damage);
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


    [AttackMethod]
    public void MeleeHit()
    {
        // Max number of entities in the OverlapSphere
        int maxColliders = 10;
        Collider[] hitColliders = new Collider[maxColliders];

        int numColliders = Physics.OverlapSphereNonAlloc(TargetPosition, meleeHitboxSize, hitColliders, characterLayer);

        for (int i = 0; i < numColliders; i++)
        {
            if (hitColliders[i].CompareTag("Player"))
            {
                hitColliders[i].GetComponent<ImpactReceiver>()?.AddImpact(hitColliders[i].transform.position - transform.position, 15);
                EventManager.PlayerTakeDamage(currentAbility.damage);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (target == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(TargetPosition, meleeHitboxSize);
    }

    // Triggered via SpawnAoe Attack Animation
    [AttackMethod]
    public void SpawnAOE()
    {
        if (!target) return;

        GameObject newObject = PoolingManagerSingleton.Instance.GetObjectFromPool(currentAbility.projectilePrefab.name, TargetPosition + new Vector3(0, 0.2f, 0));

        if (newObject.TryGetComponent<AbilityValues>(out AbilityValues aoeSpell))
        {
            aoeSpell.Damage = currentAbility.damage;
            aoeSpell.DoDamage(currentAbility.damage);
        }
    }


    [AttackMethod]
    public void RangedHit()
    {
        if (targetPosition == Vector3.zero) return;

        Vector3 adjustedTargetPosition = TargetPosition + new Vector3(0, 1f, 0);

        Vector3 direction = (adjustedTargetPosition - exitPoint.transform.position).normalized;

        GameObject newObject = PoolingManagerSingleton.Instance.GetObjectFromPool(currentAbility.projectilePrefab.name, exitPoint.transform.position);

        if (newObject != null)
        {
            if (newObject.TryGetComponent<AbilityValues>(out AbilityValues projectile))
            {
                projectile.Damage = currentAbility.damage;
            }

            Quaternion rotationToTarget = Quaternion.LookRotation(direction);
            newObject.transform.rotation = rotationToTarget;
        }
    }



    [AttackMethod]
    public void BossRockFall()
    {
        EventManager.BossRockFall(1);
        //EventManager.MessageEvent("Hide!", 3);
    }

    [AttackMethod]
    public void TestSpellIndicator()
    {
        bool canFallingRocksDestroyDestructibles = true; // Set this based on your game logic, if rocks can destroy destructible obstacles
        //Vector3 targetPos = GetInaccurateTarget(target.position);
        Vector3 groundTargetPosition = TargetPosition + new Vector3(0, 0.01f, 0);
        RockFallAtTargetPos(2f, 3f, groundTargetPosition, canFallingRocksDestroyDestructibles);
    }

    private void RockFallAtTargetPos(float duration, float size, Vector3 targetPos, bool canDestroy)
    {
        GameObject newObject = PoolingManagerSingleton.Instance.GetObjectFromPool("Telegraph_AoE", targetPos);

        if (newObject.TryGetComponent<TelegraphIndicator>(out TelegraphIndicator indicator))
        {
            indicator.SetIndicatorPosition(duration, size);
            StartCoroutine(RockFalling(duration, size, targetPos, canDestroy));
        }
    }


    private IEnumerator RockFalling(float indicatorDuration, float size, Vector3 targetPos, bool canDestroy)
    {
        float desiredFallDuration = 1f;

        float waitTime = indicatorDuration - desiredFallDuration;
        yield return new WaitForSeconds(waitTime);

        GameObject newObject = PoolingManagerSingleton.Instance.GetObjectFromPool("Rock_Cylinder", targetPos + Vector3.up * 10f);

        Vector3 startPos = newObject.transform.position;
        float halfHeight = newObject.transform.localScale.y * 0.5f;
        Vector3 endPos = targetPos + Vector3.up * halfHeight + new Vector3(0, 0.5f, 0);

        float elapsed = 0f;

        // Make the rock fall in exactly 1 second
        while (elapsed < 1f)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / 1f); 

            float curvedT = Mathf.Pow(t, 4);

            newObject.transform.position = Vector3.Lerp(startPos, endPos, curvedT);

            yield return null;
        }

        newObject.transform.position = endPos;
        newObject.GetComponent<RockMaterialFade>().StartCoroutine("RockFalling");

        // overlap sphere to check for characters
        int maxColliders = 10;
        Collider[] hitColliders = new Collider[maxColliders];
        int numColliders = Physics.OverlapSphereNonAlloc(newObject.transform.position, size/2, hitColliders, characterLayer);

        for (int i = 0; i < numColliders; i++)
        {
            if (hitColliders[i].CompareTag("Player"))
            {
                // if we want to add a pushback to player
                //hitColliders[i].GetComponent<ImpactReceiver>()?.AddImpact(hitColliders[i].transform.position - newObject.transform.position, 50);
                EventManager.PlayerTakeDamage(currentAbility.damage);
            }
        }

        if (canDestroy)
        {
            // overlap sphere to check for rocks
            int maxRockColliders = 5;
            Collider[] hitRocksColliders = new Collider[maxRockColliders];
            int numRocksColliders = Physics.OverlapSphereNonAlloc(newObject.transform.position, size/2, hitRocksColliders, LayerMask.GetMask("Obstacle"));

            for (int i = 0; i < numRocksColliders; i++)
            {
                if (hitRocksColliders[i].CompareTag("Destructible"))
                {
                    hitRocksColliders[i].gameObject.SetActive(false);
                }
            }
        }

        yield return new WaitForSeconds(2f);

    }


    [AttackMethod]
    public void TripleRockFallFront()
    {
        StartCoroutine(TripleRockFallRoutine());
    }

    private IEnumerator TripleRockFallRoutine()
    {
        bool canFallingRocksDestroyDestructibles = true; // Set this based on your game logic, if rocks can destroy destructible obstacles
        float spacing = 2f;

        for (int i = 0; i < 3; i++)
        {
            //Vector3 targetPos = GetInaccurateTarget(target.position);
            Vector3 basePosition = TargetPosition + new Vector3(0, 0.01f, 0);

            // Generate a random offset in 2D space (XZ plane)
            Vector2 randomOffset = Random.insideUnitCircle * spacing;

            // Apply the offset to the base position
            Vector3 spawnPosition = new Vector3(
                target.position.x + randomOffset.x,
                target.position.y,
                target.position.z + randomOffset.y
            );

            float indicatorDuration = .5f + (i * 0.25f);
            float size = 2f + (i * 0.75f);

            RockFallAtTargetPos(indicatorDuration, size, spawnPosition, canFallingRocksDestroyDestructibles);

            yield return new WaitForSeconds(0.5f); // Small delay between rocks
        }
    }

    [AttackMethod]
    public void ChargingUntilObstacleStart()
    {
        if (target == null) return;
        //Vector3 targetPos = GetInaccurateTarget(target.position);
        Vector3 chargeDirection = (TargetPosition - transform.position).normalized;
        StartCoroutine(ChargeUntilObstacleCoroutine(chargeDirection));
    }

    private IEnumerator ChargeUntilObstacleCoroutine(Vector3 chargeDirection)
    {
        bool canFallingRocksDestroyDestructibles = false; // Set this based on your game logic, if rocks can destroy destructible obstacles

        isCharging = true;

        transform.forward = chargeDirection;

        agent.avoidancePriority = 0;
        agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
        agent.updateRotation = false;

        HashSet<Collider> alreadyHitPlayers = new HashSet<Collider>();

        while (true)
        {
            agent.Move(chargeDirection * speed * 2.25f * Time.deltaTime);

            float characterHeight = 2.25f;
            float radius = 1f;

            Vector3 capsuleStart = transform.position + Vector3.up * (characterHeight - radius);
            Vector3 capsuleEnd = transform.position + Vector3.up * radius;

            Collider[] hits = Physics.OverlapCapsule(capsuleStart, capsuleEnd, radius, LayerMask.GetMask("Character", "Obstacle"));

            foreach (Collider hit in hits)
            {
                if (hit.CompareTag("Player") && !alreadyHitPlayers.Contains(hit))
                {
                    alreadyHitPlayers.Add(hit);
                    OnHitPlayer(hit);
                }
                else if (hit.CompareTag("Destructible"))
                {
                    OnHitDestructibleObstacle(hit);
                    cCDuration += 2.5f;
                    yield return BreakCharge();
                    yield break;
                }
                else if (hit.CompareTag("Indestructible"))
                {
                    OnHitIndestructibleObstacle(hit, canFallingRocksDestroyDestructibles);
                    cCDuration += 2.5f;
                    yield return BreakCharge();
                    yield break;
                }
            }

            yield return null; // wait for next frame
        }
    }


    private IEnumerator BreakCharge()
    {
        isCharging = false;
        ResetAttackingAndPowering();

        agent.avoidancePriority = 50;
        agent.obstacleAvoidanceType = ObstacleAvoidanceType.LowQualityObstacleAvoidance;
        agent.updateRotation = true;

        agent.Move(Vector3.zero);

        yield return null;
    }


    private void OnHitPlayer(Collider player)
    {
        EventManager.PlayerTakeDamage(currentAbility.damage);
    }

    private void OnHitDestructibleObstacle(Collider obstacle)
    {
        obstacle.gameObject.SetActive(false);
    }

    private void OnHitIndestructibleObstacle(Collider obstacle, bool canFallingRocksDestroyDestructibles)
    {
        Vector3 start = BossFightManager.Instance.BottomLeftCorner.position;
        Vector3 end = BossFightManager.Instance.TopRightCorner.position;

        // THE SPACING BETWEEN ROCKS IS RESPONSIBLE FOR THE AMOUNT OF ROCKS SPAWNED
        float spacing = 4; // spacing between the rocks

        List<Vector3> positions = new List<Vector3>();

        for (float x = start.x; x <= end.x; x += spacing)
        {
            for (float z = start.z; z <= end.z; z += spacing)
            {
                positions.Add(new Vector3(x, start.y, z));
            }
        }

        foreach (Vector3 pos in positions)
        {
            StartCoroutine(DelayedRockFall(pos, canFallingRocksDestroyDestructibles));
        }
    }
    private IEnumerator DelayedRockFall(Vector3 pos, bool canRockDestroyDestructible)
    {
        int delay = Random.Range(0, 3);
        yield return new WaitForSeconds(delay);

        int duration = Random.Range(3, 7);
        int size = Random.Range(3, 7);

        // Add random offset to position
        float offsetX = Random.Range(-2f, 2f);
        float offsetZ = Random.Range(-2f, 2f);

        Vector3 randomizedPos = new Vector3(pos.x + offsetX, pos.y, pos.z + offsetZ);

        RockFallAtTargetPos(duration, size, pos, canRockDestroyDestructible);
    }

    private IEnumerator TriggerAbilityAfterDelay(float delay, Vector3 targetPos)
    {
        yield return new WaitForSeconds(delay);

        TriggerAbility(targetPos);
    }

    private void TriggerAbility(Vector3 targetPos)
    {
        GameObject newObject = PoolingManagerSingleton.Instance.GetObjectFromPool(currentAbility.projectilePrefab.name, targetPos + new Vector3(0, 0.2f, 0));

        if (newObject.TryGetComponent<AbilityValues>(out AbilityValues aoeSpell))
        {
            aoeSpell.Damage = currentAbility.damage;
            aoeSpell.DoDamage(currentAbility.damage);
        }
    }


}
