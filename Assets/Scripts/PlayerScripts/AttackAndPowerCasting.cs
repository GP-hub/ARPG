using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class AttackAndPowerCasting : MonoBehaviour
{
    [Header("Spells")]
    [SerializeField] private GameObject exitPoint;
    [SerializeField] private LayerMask groundLayer;
    private string firewallPrefabName;
    private float firewallDamagePerTick;
    private int ultimateBuffCount = 0;
    public bool isBuffedUltimate => ultimateBuffCount > 0;

    [Space(10)]
    [Header("Attack")]
    [SerializeField] private DecalProjector attackSpellIndicator;
    [SerializeField] private string attackPrefabName;
    [SerializeField] private float baseAttackDamage;
    private float currentAttackDamage;
    [SerializeField] private float attackCCDuration;
    // the player attack projectile which is the fireball, has its speed dictated by the fireball prefab itself
    //[SerializeField] private float attackProjectileSpeed = 10f;
    [SerializeField] private float attackCooldownTime;
    [SerializeField] private float attackPlayerMovementSpeedPercent;
    [SerializeField] private float attackSpeedMultiplier;
    [SerializeField] private Image attackCooldownImage;

    [Space(10)]
    [Header("Power")]

    [SerializeField] private DecalProjector powerSpellIndicator;
    [SerializeField] private PowerIndicatorGrowth secondaryIndicator;
    [SerializeField] private AnimationClip powerAnimationClip;
    private float basePowerCastTime;
    [SerializeField] private string powerPrefabName;
    [SerializeField] private string firePoolName;
    [SerializeField] private float firePoolDamage;
    [SerializeField] private float basePowerDamage;
    private float currentPowerDamage;
    [SerializeField] private float powerCCDuration;
    [SerializeField] private float powerCooldownTime;
    [SerializeField] private float powerPlayerMovementSpeedPercent;
    [SerializeField] private float powerSpeedMultiplier;
    [SerializeField] private Image powerCooldownImage;


    private PlayerInput playerInput;

    private bool isAlive;
    private bool canCast = true;

    private bool isAttackingHeldDown = false;
    private bool isPoweringHeldDown = false;
    private bool isCasting = false;
    private bool isDashing = false;

    private bool isAttackCooldown = false;
    private bool isPowerCooldown = false;

    private float attackCooldownTimeElapsed;
    private float powerCooldownTimeElapsed;

    private PlayerMovement playerMovement;
    private PlayerHealth playerHealth;

    private Animator animator;
    [SerializeField] private Transform lineDecal;

    public bool IsCasting { get => isCasting; }
    public float AttackDamage { get => baseAttackDamage; set => baseAttackDamage = value; }
    public float PowerDamage { get => basePowerDamage; set => basePowerDamage = value; }
    public string FireballPrefabName { get => attackPrefabName; set => attackPrefabName = value; }

    private void Awake()
    {
        playerHealth = GetComponent<PlayerHealth>();
        playerMovement = GetComponent<PlayerMovement>();
        playerInput = GetComponent<PlayerInput>();
        animator = GetComponent<Animator>();

        playerInput.actions.FindAction("Attack").performed += OnAttackChanged;
        playerInput.actions.FindAction("Attack").canceled += OnAttackChanged;

        playerInput.actions.FindAction("Power").performed += OnPowerChanged;
        playerInput.actions.FindAction("Power").canceled += OnPowerChanged;
    }

    private void Start()
    {
        EventManager.onEnemyTakeDamage += DoDamage;
        EventManager.onEnemyGetCC += ApplyCCDuration;
        SpellCharge.InitializeSpellCharge();

        basePowerCastTime = GetLastEventTime();
    }
    public float GetLastEventTime()
    {
        if (powerAnimationClip == null || powerAnimationClip.events.Length == 0)
            return -1f;

        float maxTime = float.MinValue;
        foreach (AnimationEvent e in powerAnimationClip.events)
        {
            if (e.time > maxTime)
                maxTime = e.time;
        }
        Debug.Log($"Last event time in {powerAnimationClip.name}: {maxTime} seconds");
        return maxTime;
    }

    private void OnEnable()
    {
        EventManager.onDashing += Dashing;
        EventManager.onPlayerDeath += HandleAimingLayerWeight;
    }

    private void OnDestroy()
    {
        EventManager.onPlayerDeath -= HandleAimingLayerWeight;
        EventManager.onEnemyTakeDamage -= DoDamage;
        EventManager.onEnemyGetCC -= ApplyCCDuration;
        EventManager.onDashing -= Dashing;
    }

    private void OnDisable()
    {
        EventManager.onDashing -= Dashing;
        EventManager.onPlayerDeath -= HandleAimingLayerWeight;
    }

    private void Update()
    {
        HandlingCasting();
        CheckAnimationState();
    }

    private void HandlingCasting()
    {
        if (!playerHealth.IsAlive) return;

        if (isCasting) return;

        // If we want to be able to cast while dashing
        //if (isDashing) return;

        if (isAttackingHeldDown && !isAttackCooldown && canCast)
        {
            CastAttack();
            return;
        }

        if (isPoweringHeldDown)
        {
            if (!isPowerCooldown)
            {
                CastPower();
            }
        }
    }

    private bool IsAnimationPlaying(string stateName)
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(1);
        return stateInfo.IsName(stateName) && stateInfo.normalizedTime < 1f && !animator.IsInTransition(1);
    }
    private void CheckAnimationState()
    {
        if (IsAnimationPlaying("Attack"))
        {
            canCast = false;
            return;
        }

        if (animator.GetCurrentAnimatorStateInfo(1).IsName("Attack"))
        {
            canCast = true;
        }
    }

    private void HandleAimingLayerWeight()
    {
        int aimingLayerIndex = animator.GetLayerIndex("Aiming");
        if (aimingLayerIndex >= 0)
        {
            animator.SetLayerWeight(aimingLayerIndex, 0f);
        }
    }

    private void Dashing(bool dashing)
    {
        if (string.IsNullOrEmpty(firewallPrefabName))
        {
            Firewall firewall = GetComponent<Firewall>();
            firewallPrefabName = firewall.FirewallPrefabName;
            firewallDamagePerTick = firewall.GetFirewallDamagePerTick();
        }

        isDashing = dashing;
    }

    private void OnAttackChanged(InputAction.CallbackContext context)
    {
        if (context.performed) isAttackingHeldDown = true;
        else if (context.canceled) isAttackingHeldDown = false;
    }

    private void OnPowerChanged(InputAction.CallbackContext context)
    {
        if (context.performed) isPoweringHeldDown = true;
        else if (context.canceled) isPoweringHeldDown = false;
    }

    private void CastAttack()
    {
        isCasting = true;
        isAttackingHeldDown = true;
        EventManager.Casting(true);
        animator.SetTrigger("Attack");
        attackSpellIndicator.fadeFactor = 1;

        StartCoroutine(CooldownAttackCoroutine(attackCooldownTime));
    }


    // Triggered by the end of the Attack animation
    public void ResetAttack()
    {
        EventManager.Casting(false);
        isCasting = false;
        animator.ResetTrigger("Attack");
        playerMovement.RemoveSpeedModifier("Attack");
    }

    // Trigger by first keyframe of Attack animation
    public void MoveSpeedPlayerOnAttack()
    {
        //float targetSpeed = playerMovement.PlayerSpeed * (1f - (attackPlayerMovementSpeedPercent / 100f));
        //StartCoroutine(playerMovement.RestoreSpeedCoroutine(.2f, targetSpeed, playerMovement.CurrentPlayerSpeed));
        playerMovement.AddSpeedModifier("Attack", attackPlayerMovementSpeedPercent);

        animator.SetFloat("AttackSpeed", attackSpeedMultiplier);
    }

    public void BuffByUltimate()
    {
        ultimateBuffCount++;
    }
    public void RemoveUltimateBuff()
    {
        ultimateBuffCount = Mathf.Max(0, ultimateBuffCount - 1);
    }


    private void CastPower()
    {
        EventManager.Casting(true);
        isCasting = true;

        isPoweringHeldDown = true;
        animator.SetTrigger("Power");
        powerSpellIndicator.fadeFactor = 1;
        InnerPowerSpellIndicatorGrowth();

        StartCoroutine(CooldownPowerCoroutine(powerCooldownTime));
    }

    private void InnerPowerSpellIndicatorGrowth()
    {
        powerSpeedMultiplier = (SpellCharge.SpellCount == 0) ? .5f : SpellCharge.SpellCount;
        float castTime = basePowerCastTime / powerSpeedMultiplier;

        secondaryIndicator.StartGrowth(castTime);
    }

    // Trigger by first keyframe of Power animation
    public void MoveSpeedPlayerOnPower()
    {
        //float targetSpeed = playerMovement.PlayerSpeed * (1f - (powerPlayerMovementSpeedPercent / 100f));
        //StartCoroutine(playerMovement.RestoreSpeedCoroutine(.2f, targetSpeed, playerMovement.CurrentPlayerSpeed));
        playerMovement.AddSpeedModifier("Power", powerPlayerMovementSpeedPercent);

        // IF spellCount is 0 THEN powerSpeed is 1, ELSE powerSpeed is equal to spellCount
        powerSpeedMultiplier = (SpellCharge.SpellCount == 0) ? .5f : SpellCharge.SpellCount;

        animator.SetFloat("PowerSpeed", powerSpeedMultiplier);
    }

    private IEnumerator CooldownAttackCoroutine(float cd)
    {
        isAttackCooldown = true;
        attackCooldownTimeElapsed = 0f;
        attackCooldownImage.fillAmount = 1;


        while (attackCooldownTimeElapsed < cd)
        {
            attackCooldownTimeElapsed += Time.deltaTime;
            attackCooldownImage.fillAmount = 1 - attackCooldownTimeElapsed / attackCooldownTime;
            yield return null;
        }
        attackCooldownImage.fillAmount = 0;
        isAttackCooldown = false;
    }

    private IEnumerator CooldownPowerCoroutine(float cd)
    {
        isPowerCooldown = true;
        powerCooldownTimeElapsed = 0f;
        powerCooldownImage.fillAmount = 1;

        while (powerCooldownTimeElapsed < cd)
        {
            powerCooldownTimeElapsed += Time.deltaTime;
            powerCooldownImage.fillAmount = 1 - powerCooldownTimeElapsed / powerCooldownTime;
            yield return null;
        }
        powerCooldownImage.fillAmount = 0;
        isPowerCooldown = false;
    }

    //private Vector3 CorrectingAimPosition(Vector3 hit)
    //{
    //    Vector3 pointA = hit;
    //    Vector3 pointB = Camera.main.transform.position;
    //    Vector3 pointC = new Vector3(Camera.main.transform.position.x, hit.y, Camera.main.transform.position.z);

    //    float squaredLengthAB = (pointB - pointA).sqrMagnitude;
    //    float squaredLengthBC = (pointC - pointB).sqrMagnitude;
    //    float squaredLengthCA = (pointA - pointC).sqrMagnitude;

    //    float lenghtHypotenuse = Mathf.Sqrt(squaredLengthAB);
    //    float lengthBC = Mathf.Sqrt(squaredLengthBC);
    //    float lengthCA = Mathf.Sqrt(squaredLengthCA);

    //    float angleAtHit = CalculateAngle(lengthCA, lenghtHypotenuse, lengthBC);

    //    float angleNextToHit = 90 - angleAtHit;

    //    Vector3 direction = (Camera.main.transform.position - hit).normalized;
    //    float distance = CalculateSideLengths(angleNextToHit);
    //    Vector3 targetPosition = pointA + direction * distance;

    //    return targetPosition;
    //}

    //private float CalculateAngle(float sideA, float sideB, float sideC)
    //{
    //    return Mathf.Acos((sideA * sideA + sideB * sideB - sideC * sideC) / (2 * sideA * sideB)) * Mathf.Rad2Deg;
    //}

    //private float CalculateSideLengths(float angleA)
    //{
    //    float sideB = exitPoint.transform.position.y;

    //    float radianA = angleA * Mathf.Deg2Rad;

    //    float sideA = sideB * Mathf.Tan(radianA);

    //    return sideA;
    //}
    

    public void CastFireball()
    {
        float yRotation = lineDecal.transform.eulerAngles.y;
 
        Vector3 direction = Quaternion.Euler(0, yRotation + 90, 0) * Vector3.forward;
        //Vector3 direction = Quaternion.Euler(0, this.transform.eulerAngles.y, 0) * Vector3.forward;
        currentAttackDamage = (SpellCharge.SpellCount == 0) ? baseAttackDamage : (SpellCharge.SpellCount * baseAttackDamage) + baseAttackDamage;

        GameObject newObject = PoolingManagerSingleton.Instance.GetObjectFromPool(attackPrefabName, exitPoint.transform.position);

        if (newObject != null)
        {
            newObject.transform.rotation = Quaternion.LookRotation(direction);
        }
        attackSpellIndicator.fadeFactor = 0;
        //EventManager.Casting(false);
        //isCasting = false;
        //animator.ResetTrigger("Attack");
        //attackSpellIndicator.fadeFactor = 0;
    }

    // Called by Player Power Animation Keyframe
    public void CastMeteor()
    {
        Ray cursorRay = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(cursorRay, out RaycastHit hit, 100f, groundLayer))
        {
            Vector3 targetPosition = hit.point;
            Vector3 spawnPosition = targetPosition + new Vector3(0, 0.1f, 0); //Offset slightly above the ground
            GameObject newObject = PoolingManagerSingleton.Instance.GetObjectFromPool(powerPrefabName, spawnPosition);

            if (newObject != null)
            {
                currentPowerDamage = (SpellCharge.SpellCount == 0) ? basePowerDamage : (SpellCharge.SpellCount * basePowerDamage) + basePowerDamage;

                SpellCharge.ResetSpellCount();
                StartCoroutine(DelayedMeteorExplosion(newObject, .825f, spawnPosition));
            }
        }

        EventManager.Casting(false);
        isCasting = false;
        animator.ResetTrigger("Power");
        powerSpellIndicator.fadeFactor = 0;
        playerMovement.RemoveSpeedModifier("Power");
    }

    private IEnumerator DelayedMeteorExplosion(GameObject meteorObject, float delay, Vector3 spawnPosition)
    {
        yield return new WaitForSeconds(delay);
        meteorObject.GetComponent<Meteor>().Explode();
        Debug.Log("Meteor Exploded: " + isBuffedUltimate);
        if (isBuffedUltimate)
        {
            Debug.Log("Patch fire");
            GameObject newObject = PoolingManagerSingleton.Instance.GetObjectFromPool(firePoolName, spawnPosition);
        }
    }


    private void DoDamage(Enemy enemy, string skill)
    {
        if (skill.ToLower().Contains(attackPrefabName.ToLower()))
        {
            enemy.TakeDamage(currentAttackDamage);
        }
        else if (skill.ToLower().Contains(powerPrefabName.ToLower()))
        {
            enemy.TakeDamage(currentPowerDamage);
        }
        else if (skill.ToLower().Contains(firePoolName.ToLower()))
        {
            enemy.TakeDamage(firePoolDamage);
        }
        else if (skill.ToLower().Contains(firewallPrefabName.ToLower()))
        {
            enemy.TakeDamage(firewallDamagePerTick);
        }
    }

    private void ApplyCCDuration(Enemy enemy, string skill)
    {
        if (skill.ToLower().Contains(attackPrefabName.ToLower()))
        {
            enemy.UpdateCCDuration(attackCCDuration, this.gameObject.tag);
        }
        else if (skill.ToLower().Contains(powerPrefabName.ToLower()))
        {
            enemy.UpdateCCDuration(powerCCDuration, this.gameObject.tag);
        }
    }
}

