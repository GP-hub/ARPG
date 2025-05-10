using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class AttackAndPowerCasting : MonoBehaviour
{
    [Header("Spells")]
    [SerializeField] private GameObject exitPoint;
    [SerializeField] private LayerMask groundLayer;


    [Space(10)]
    [Header("Attack")]
    [SerializeField] private DecalProjector attackSpellIndicator;
    [SerializeField] private string attackPrefabName;
    [SerializeField] private float attackDamage = 10f;
    [SerializeField] private float attackCCDuration = 3f;
    // the player attack projectile which is the fireball, has its speed dictated by the fireball prefab itself
    //[SerializeField] private float attackProjectileSpeed = 10f;
    [SerializeField] private float attackCooldownTime = 1f;
    [SerializeField] private float attackPlayerMovementSpeedPercent = 2;
    [SerializeField] private float attackSpeedMultiplier = 1;
    [SerializeField] private Image attackCooldownImage;

    [Space(10)]
    [Header("Power")]
    [SerializeField] private DecalProjector powerSpellIndicator;
    [SerializeField] private string powerPrefabName;
    [SerializeField] private float powerDamage = 5f;
    [SerializeField] private float powerCCDuration = 5f;
    [SerializeField] private float powerCooldownTime = 5f;
    [SerializeField] private float powerPlayerMovementSpeedPercent = 5;
    [SerializeField] private float powerSpeedMultiplier = 1;
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
    public float AttackDamage { get => attackDamage; set => attackDamage = value; }
    public float PowerDamage { get => powerDamage; set => powerDamage = value; }
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
    }

    private void OnEnable()
    {
        EventManager.onDashing += Dashing;
    }

    private void OnDestroy()
    {
        EventManager.onEnemyTakeDamage -= DoDamage;
        EventManager.onEnemyGetCC -= ApplyCCDuration;
        EventManager.onDashing -= Dashing;
    }

    private void OnDisable()
    {
        EventManager.onDashing -= Dashing;
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

    private void Dashing(bool dashing)
    {
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
    }

    // Trigger by first keyframe of Attack animation
    public void MoveSpeedPlayerOnAttack()
    {
        // Speed of the player when casting attack
        playerMovement.PlayerSpeed -= attackPlayerMovementSpeedPercent;

        // Animation speed when using attacking
        animator.SetFloat("AttackSpeed", attackSpeedMultiplier);
    }

    private void CastPower()
    {
        EventManager.Casting(true);
        isCasting = true;

        isPoweringHeldDown = true;
        animator.SetTrigger("Power");
        powerSpellIndicator.fadeFactor = 1;

        StartCoroutine(CooldownPowerCoroutine(powerCooldownTime));
    }

    // Trigger by first keyframe of Power animation
    public void MoveSpeedPlayerOnPower()
    {
        // Speed of the player when casting power
        playerMovement.PlayerSpeed -= powerPlayerMovementSpeedPercent;

        // IF spellCount is 0 THEN powerSpeed is 1, ELSE powerSpeed is equal to spellCount
        powerSpeedMultiplier = (SpellCharge.SpellCount == 0) ? 1 : SpellCharge.SpellCount;

        // Animation speed when using power
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
        // We return the player speed to its original value
        playerMovement.PlayerSpeed += attackPlayerMovementSpeedPercent;

        #region previous method
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////      OLD WAY      ////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////

        //// Define the plane at the current height of the projectile above ground
        //Plane aimingPlane = new Plane(Vector3.up, new Vector3(0, exitPoint.transform.position.y, 0));

        //// Raycast from camera to mouse cursor on that plane
        //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        //if (aimingPlane.Raycast(ray, out float distance))
        //{
        //    // Get the cursor position on that plane, keeping the Y value fixed to the projectile's height
        //    Vector3 targetPoint = ray.GetPoint(distance);
        //    targetPoint.y = exitPoint.transform.position.y;  // Fix height to projectile's current hover height

        //    // Calculate the direction towards the target point on the plane
        //    Vector3 direction = (targetPoint - exitPoint.transform.position).normalized;

        //    // Spawn the projectile and set its direction
        //    GameObject newObject = PoolingManagerSingleton.Instance.GetObjectFromPool(attackPrefabName, exitPoint.transform.position);

        //    if (newObject != null)
        //    {
        //        newObject.transform.rotation = Quaternion.LookRotation(direction);
        //        Debug.DrawLine(exitPoint.transform.position, targetPoint, Color.magenta, 2f); // Visualize the direction
        //    }
        //}

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #endregion previous method

        float yRotation = lineDecal.transform.eulerAngles.y;
 
        Vector3 direction = Quaternion.Euler(0, yRotation + 90, 0) * Vector3.forward;
        //Vector3 direction = Quaternion.Euler(0, this.transform.eulerAngles.y, 0) * Vector3.forward;

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
        playerMovement.PlayerSpeed += powerPlayerMovementSpeedPercent;

        Ray cursorRay = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(cursorRay, out RaycastHit hit, 100f, groundLayer))
        {
            Vector3 targetPosition = hit.point;

            GameObject newObject = PoolingManagerSingleton.Instance.GetObjectFromPool(powerPrefabName, targetPosition);

            if (newObject != null)
            {
                SpellCharge.ResetSpellCount();

                // Trying to make the meteor explode when we cast them
                newObject.GetComponent<Blackhole>().Explode();
            }
        }

        EventManager.Casting(false);
        isCasting = false;
        animator.ResetTrigger("Power");
        powerSpellIndicator.fadeFactor = 0;
    }


    private void DoDamage(Enemy enemy, string skill)
    {
        if (skill.ToLower().Contains(attackPrefabName.ToLower()))
        {
            enemy.TakeDamage(attackDamage);
        }
        else if (skill.ToLower().Contains(powerPrefabName.ToLower()))
        {
            enemy.TakeDamage(powerDamage);
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

