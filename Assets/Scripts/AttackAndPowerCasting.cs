using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class AttackAndPowerCasting : MonoBehaviour
{
    [Header("Spells")]
    [SerializeField] private GameObject exitPoint;
    [SerializeField] private LayerMask groundLayer;


    [Space(10)]
    [Header("Attack")]
    [SerializeField] private string fireballPrefabName;
    [SerializeField] private float attackDamage = 10f;
    [SerializeField] private float attackProjectileSpeed = 10f;
    [SerializeField] private float attackCooldownTime = 1f;
    [SerializeField] private float attackPlayerMovementSpeedPercent = 2;
    [SerializeField] private float attackSpeedMultiplier = 1;
    [SerializeField] private Image attackCooldownImage;

    [Space(10)]
    [Header("Power")]
    [SerializeField] private string meteorPrefabName;
    [SerializeField] private float powerDamage = 5f;
    [SerializeField] private float powerCooldownTime = 5f;
    [SerializeField] private float powerPlayerMovementSpeedPercent = 5;
    [SerializeField] private float powerSpeedMultiplier = 1;
    [SerializeField] private Image powerCooldownImage;


    private PlayerInput playerInput;

    private bool isAlive;

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

    public bool IsCasting { get => isCasting;}

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
        EventManager.Instance.onEnemyTakeDamage += DoDamage;
    }

    private void OnEnable()
    {
        EventManager.Instance.onDashing += Dashing;
    }

    private void Update()
    {
        HandlingCasting();
    }

    private void HandlingCasting()
    {
        if (!playerHealth.IsAlive) return;

        if (isCasting) return;

        // If we want to be able to cast while dashing
        //if (isDashing) return;

        if (isAttackingHeldDown && !isAttackCooldown)
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
        EventManager.Instance.Casting(true);
        isCasting = true;

        isAttackingHeldDown = true;
        animator.SetTrigger("Attack");
        StartCoroutine(CooldownAttackCoroutine(attackCooldownTime));
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
        EventManager.Instance.Casting(true);
        isCasting = true;

        isPoweringHeldDown = true;
        animator.SetTrigger("Power");
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

    private Vector3 CorrectingAimPosition(Vector3 hit)
    {
        Vector3 pointA = hit;
        Vector3 pointB = Camera.main.transform.position;
        Vector3 pointC = new Vector3(Camera.main.transform.position.x, hit.y, Camera.main.transform.position.z);

        float squaredLengthAB = (pointB - pointA).sqrMagnitude;
        float squaredLengthBC = (pointC - pointB).sqrMagnitude;
        float squaredLengthCA = (pointA - pointC).sqrMagnitude;

        float lenghtHypotenuse = Mathf.Sqrt(squaredLengthAB);
        float lengthBC = Mathf.Sqrt(squaredLengthBC);
        float lengthCA = Mathf.Sqrt(squaredLengthCA);

        float angleAtHit = CalculateAngle(lengthCA, lenghtHypotenuse, lengthBC);

        float angleNextToHit = 90 - angleAtHit;

        Vector3 direction = (Camera.main.transform.position - hit).normalized;
        float distance = CalculateSideLengths(angleNextToHit);
        Vector3 targetPosition = pointA + direction * distance;

        return targetPosition;
    }

    private float CalculateAngle(float sideA, float sideB, float sideC)
    {
        return Mathf.Acos((sideA * sideA + sideB * sideB - sideC * sideC) / (2 * sideA * sideB)) * Mathf.Rad2Deg;
    }

    private float CalculateSideLengths(float angleA)
    {
        float sideB = exitPoint.transform.position.y;

        float radianA = angleA * Mathf.Deg2Rad;

        float sideA = sideB * Mathf.Tan(radianA);

        return sideA;
    }

    // Called by Player Attack Animation Keyframe
    public void CastFireball()
    {
        // We return the player speed to its original value
        playerMovement.PlayerSpeed += attackPlayerMovementSpeedPercent;
        animator.ResetTrigger("Attack");

        Ray cursorRay = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(cursorRay, out RaycastHit hit, 100f, groundLayer))
        {
            Vector3 targetPosition = hit.point;
            
            Vector3 targetCorrectedPosition = new Vector3(CorrectingAimPosition(targetPosition).x, targetPosition.y, CorrectingAimPosition(targetPosition).z);
            Vector3 direction = (targetCorrectedPosition - this.transform.position).normalized;

            GameObject newObject = PoolingManagerSingleton.Instance.GetObjectFromPool(fireballPrefabName, exitPoint.transform.position);

            if (newObject != null)
            {
                Quaternion rotationToTarget = Quaternion.LookRotation(direction);
                newObject.transform.rotation = rotationToTarget;
            }
        }

        EventManager.Instance.Casting(false);
        isCasting = false;
    }

    // Called by Player Power Animation Keyframe
    public void CastMeteor()
    {
        playerMovement.PlayerSpeed += powerPlayerMovementSpeedPercent;
        animator.ResetTrigger("Power");

        Ray cursorRay = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(cursorRay, out RaycastHit hit, 100f, groundLayer))
        {
            Vector3 targetPosition = hit.point;

            GameObject newObject = PoolingManagerSingleton.Instance.GetObjectFromPool(meteorPrefabName, targetPosition);

            if (newObject != null)
            {
                SpellCharge.ResetSpellCount();

                // Trying to make the meteor explode when we cast them
                newObject.GetComponent<Blackhole>().Explode();
            }
        }

        EventManager.Instance.Casting(false);
        isCasting = false;
    }


    private void DoDamage(Enemy enemy, string skill)
    {
        if (skill.ToLower().Contains(fireballPrefabName.ToLower()))
        {
            enemy.TakeDamage(attackDamage);
        }
        else if (skill.ToLower().Contains(meteorPrefabName.ToLower()))
        {
            enemy.TakeDamage(powerDamage);
        }
    }
}

