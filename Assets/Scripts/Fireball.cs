using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class Fireball : MonoBehaviour
{
    private PlayerInput playerInput;

    private bool isAttacking = false;
    private bool isPowering = false;

    private bool isAttackCooldown = false;
    private bool isPowerCooldown = false;

    private float attackCooldownTime = 1f;
    private float powerCooldownTime = 5f;

    private TwinStickMovement twinStickMovement;

    public GameObject fireballPrefab;
    public GameObject meteorPrefab;
    public GameObject exitPoint;

    public float projectileSpeed = 10f;

    private List<GameObject> fireballObjectPool = new List<GameObject>();
    private List<GameObject> powerObjectPool = new List<GameObject>();

    public int maxObjects = 5;

    public float lifetime = 1.5f;

    private Animator animator;

    public LayerMask groundLayer;

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
    }

    private void Update()
    {
        HandlingCasting();
    }

    private void HandlingCasting()
    {
        if (isAttacking && !isPowering)
        {
            if (!isAttackCooldown)
            {
                CastAttack();
            }
            else
            {
                animator.ResetTrigger("Attack");
            }
        }

        if (isPowering)
        {
            if (!isPowerCooldown)
            {
                CastPower();
            }
        }
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
        isAttacking = true;
        animator.SetTrigger("Attack");
        StartCoroutine(CooldownAttackCoroutine(attackCooldownTime));
    }

    // Trigger by first keyframe of Attack animation
    public void MoveSpeedPlayerOnAttack()
    {
        twinStickMovement.PlayerSpeed = 2;
        animator.speed = .5f;
    }

    private void CastPower()
    {
        isPowering = true;
        animator.SetTrigger("Power");
        StartCoroutine(CooldownPowerCoroutine(powerCooldownTime));
    }

    // Trigger by first keyframe of Power animation
    public void MoveSpeedPlayerOnPower()
    {
        twinStickMovement.PlayerSpeed = 0f;
        animator.speed = 0.5f;
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
        twinStickMovement.PlayerSpeed = 5;
        animator.speed = 1;
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
                    newObjectRigidbody.velocity = direction * projectileSpeed;
                    StartCoroutine(DisableObjectAfterTime(newObject, lifetime));
                }
            }
        }
    }

    // Called by Player Power Animation Keyframe
    public void CastMeteor()
    {
        twinStickMovement.PlayerSpeed = 5;
        animator.speed = 1;
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
                    StartCoroutine(DisableObjectAfterTime(newObject, lifetime));
                }
            }
        }
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

        if (fireballObjectPool.Count < maxObjects)
        {
            GameObject newObject = Instantiate(fireballPrefab, exitPoint.transform.position, Quaternion.identity);
            newObject.SetActive(false);
            fireballObjectPool.Add(newObject);
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

        if (powerObjectPool.Count < maxObjects)
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
        for (int i = 0; i < maxObjects; i++)
        {
            GameObject newPowerObject = Instantiate(meteorPrefab, Vector3.zero, Quaternion.identity);
            newPowerObject.SetActive(false);
            powerObjectPool.Add(newPowerObject);
        }
    }

    private void PoolingFireballObject()
    {
        for (int i = 0; i < maxObjects; i++)
        {
            GameObject newFireballObject = Instantiate(fireballPrefab, Vector3.zero, Quaternion.identity);
            newFireballObject.SetActive(false);
            fireballObjectPool.Add(newFireballObject);
        }
    }

}

