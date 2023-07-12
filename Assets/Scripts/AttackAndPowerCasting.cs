using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.PlayerSettings;

public class AttackAndPowerCasting : MonoBehaviour
{
    [Header("Spells")]
    public GameObject exitPoint;
    public LayerMask groundLayer;
    private int maxObjectsForPooling = 5;


    [Space(10)]
    [Header("Attack")]
    public GameObject fireballPrefab;
    public GameObject fireballExplosionPrefab;
    public float attackProjectileSpeed = 10f;
    public float attackLifetime = 1.5f;
    [SerializeField] private float attackCooldownTime = 1f;
    [SerializeField] private float attackPlayerMovementSpeedPercent = 2;
    [SerializeField] private float attackSpeedMultiplier = 1;

    [Space(10)]
    [Header("Power")]
    public GameObject meteorPrefab;
    public float powerLifetime = 1.5f;
    [SerializeField] private float powerCooldownTime = 5f;
    [SerializeField] private float powerPlayerMovementSpeedPercent = 5;
    [SerializeField] private float powerSpeedMultiplier = 1;

    private PlayerInput playerInput;

    private bool isAttacking = false;
    private bool isPowering = false;
    private bool isCasting = false;
    private bool isDashing = false;

    private bool isAttackCooldown = false;
    private bool isPowerCooldown = false;

    private TwinStickMovement twinStickMovement;

    private List<GameObject> fireballObjectPool = new List<GameObject>();
    private List<GameObject> fireballExplosionObjectPool = new List<GameObject>();
    private List<GameObject> powerObjectPool = new List<GameObject>();

    private Animator animator;

    private void Awake()
    {
        twinStickMovement = GetComponent<TwinStickMovement>();
        playerInput = GetComponent<PlayerInput>();
        animator = GetComponent<Animator>();

        playerInput.actions.FindAction("Attack").performed += OnAttackChanged;
        playerInput.actions.FindAction("Attack").canceled += OnAttackChanged;

        playerInput.actions.FindAction("Power").performed += OnPowerChanged;
        playerInput.actions.FindAction("Power").canceled += OnPowerChanged;

        PoolingMeteorObject();
        PoolingFireballObject();
        PoolingFireballExplosionObject();
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
        if (isCasting) return;
        if (isDashing) return;

        if (isAttacking && !isAttackCooldown)
        {
            CastAttack();
            return;
        }

        if (isPowering)
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
        if (context.performed) isAttacking = true;
        else if (context.canceled) isAttacking = false;
    }

    private void OnPowerChanged(InputAction.CallbackContext context)
    {
        if (context.performed) isPowering = true;
        else if (context.canceled) isPowering = false;
    }

    private void CastAttack()
    {
        EventManager.Instance.Casting(true);
        isCasting = true;

        isAttacking = true;
        animator.SetTrigger("Attack");
        StartCoroutine(CooldownAttackCoroutine(attackCooldownTime));
    }

    // Trigger by first keyframe of Attack animation
    public void MoveSpeedPlayerOnAttack()
    {
        // Speed of the player when casting attack
        twinStickMovement.PlayerSpeed -= attackPlayerMovementSpeedPercent;

        // Animation speed when using attacking
        animator.SetFloat("AttackSpeed", attackSpeedMultiplier);
    }

    private void CastPower()
    {
        EventManager.Instance.Casting(true);
        isCasting = true;

        isPowering = true;
        animator.SetTrigger("Power");
        StartCoroutine(CooldownPowerCoroutine(powerCooldownTime));
    }

    // Trigger by first keyframe of Power animation
    public void MoveSpeedPlayerOnPower()
    {
        // Speed of the player when casting power
        twinStickMovement.PlayerSpeed -= powerPlayerMovementSpeedPercent;

        // Animation speed when using power
        animator.SetFloat("PowerSpeed", powerSpeedMultiplier);
    }

    private IEnumerator CooldownAttackCoroutine(float cd)
    {
        isAttackCooldown = true;
        float attackTimeElapsed = 0f;

        while (attackTimeElapsed < cd)
        {
            attackTimeElapsed += Time.deltaTime;
            yield return null;
        }

        isAttackCooldown = false;
    }

    private IEnumerator CooldownPowerCoroutine(float cd)
    {
        isPowerCooldown = true;
        float powerTimeElapsed = 0f;

        while (powerTimeElapsed < cd)
        {
            powerTimeElapsed += Time.deltaTime;
            yield return null;
        }

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
        twinStickMovement.PlayerSpeed += attackPlayerMovementSpeedPercent;
        animator.ResetTrigger("Attack");

        Ray cursorRay = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(cursorRay, out RaycastHit hit, 100f, groundLayer))
        {
            Vector3 targetPosition = hit.point;
            
            Vector3 targetCorrectedPosition = new Vector3(CorrectingAimPosition(targetPosition).x, targetPosition.y, CorrectingAimPosition(targetPosition).z);
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
                    StartCoroutine(DisableObjectAfterTime(newObject, attackLifetime));
                }
            }
        }

        EventManager.Instance.Casting(false);
        isCasting = false;
    }

    // Called by Player Power Animation Keyframe
    public void CastMeteor()
    {
        twinStickMovement.PlayerSpeed += powerPlayerMovementSpeedPercent;
        animator.ResetTrigger("Power");

        Ray cursorRay = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(cursorRay, out RaycastHit hit, 100f, groundLayer))
        {
            Vector3 targetPosition = hit.point;
            GameObject newObject = GetPooledMeteorObject(targetPosition);
            if (newObject != null)
            {
                newObject.transform.position = targetPosition;
                newObject.SetActive(true);
                Rigidbody newObjectRigidbody = newObject.GetComponent<Rigidbody>();
                if (newObjectRigidbody != null)
                {
                    StartCoroutine(DisableObjectAfterTime(newObject, powerLifetime));
                }
            }
        }

        EventManager.Instance.Casting(false);
        isCasting = false;
    }

    private IEnumerator DisableObjectAfterTime(GameObject objectToDisable, float time)
    {
        yield return new WaitForSeconds(time);
        objectToDisable.SetActive(false);
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

    private GameObject GetPooledFireballExplosionObject(Vector3 pos)
    {
        for (int i = 0; i < fireballExplosionObjectPool.Count; i++)
        {
            if (!fireballExplosionObjectPool[i].activeInHierarchy)
            {
                return fireballExplosionObjectPool[i];
            }
        }

        if (fireballExplosionObjectPool.Count < maxObjectsForPooling)
        {
            GameObject newObject = Instantiate(fireballExplosionPrefab, pos, Quaternion.identity);
            newObject.SetActive(false);
            fireballExplosionObjectPool.Add(newObject);
            return newObject;
        }
        return null;
    }

    private GameObject GetPooledMeteorObject(Vector3 pos)
    {
        for (int i = 0; i < powerObjectPool.Count; i++)
        {
            if (!powerObjectPool[i].activeInHierarchy)
            {
                return powerObjectPool[i];
            }
        }

        if (powerObjectPool.Count < maxObjectsForPooling)
        {
            GameObject newObject = Instantiate(meteorPrefab, pos, Quaternion.identity);
            newObject.SetActive(false);
            powerObjectPool.Add(newObject);
            return newObject;
        }
        return null;
    }

    private void PoolingMeteorObject()
    {
        for (int i = 0; i < maxObjectsForPooling; i++)
        {
            GameObject newPowerObject = Instantiate(meteorPrefab, Vector3.zero, Quaternion.identity);
            newPowerObject.SetActive(false);
            powerObjectPool.Add(newPowerObject);
        }
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

    private void PoolingFireballExplosionObject()
    {
        for (int i = 0; i < maxObjectsForPooling; i++)
        {
            GameObject newFireballExplosionObject = Instantiate(fireballExplosionPrefab, Vector3.zero, Quaternion.identity);
            newFireballExplosionObject.SetActive(false);
            fireballObjectPool.Add(newFireballExplosionObject);
        }
    }

}

