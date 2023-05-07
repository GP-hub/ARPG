using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Fireball : MonoBehaviour
{
    private PlayerInput playerInput;
    private bool isAttacking = false;
    private bool isCooldown = false;
    private float attackCooldownTime = 1f;
    private float attackTimeElapsed = 0f;
    private TwinStickMovement twinStickMovement;
    public GameObject projectilePrefab;
    public GameObject exitPoint;
    public float projectileSpeed = 10f;
    private List<GameObject> objectPool = new List<GameObject>();
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

        for (int i = 0; i < maxObjects; i++)
        {
            GameObject newObject = Instantiate(projectilePrefab, Vector3.zero, Quaternion.identity);
            newObject.SetActive(false);
            objectPool.Add(newObject);
        }
    }

    private void Update()
    {
        if (isAttacking)
        {
            if (!isCooldown)
            {
                CastAttack();
            }
        }
    }

    private void OnAttackChanged(InputAction.CallbackContext context)
    {
        if (context.performed) isAttacking = true;
        else if (context.canceled) isAttacking = false;
    }

    private void CastAttack()
    {
        animator.SetTrigger("Attack");
        isAttacking = true;

        twinStickMovement.PlayerSpeed = 2;
        animator.speed = .5f;

        StartCoroutine(CooldownCoroutine());
    }

    private IEnumerator CooldownCoroutine()
    {
        isCooldown = true;
        attackTimeElapsed = 0f;

        while (attackTimeElapsed < attackCooldownTime)
        {
            attackTimeElapsed += Time.deltaTime;
            yield return null;
        }

        isCooldown = false;
    }


    // Called by Player Attack Animation Keyframe
    public void MoveObject()
    {
        Ray cursorRay = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(cursorRay, out RaycastHit hit, 100f, groundLayer))
        {
            Vector3 targetPosition = hit.point/* + new Vector3(0, 1.25f, 0)*/;
            Vector3 direction = (targetPosition - /*exitPoint.*/this.transform.position).normalized;
            Debug.Log("hit point:" + direction);
            GameObject newObject = GetPooledObject();
            if (newObject != null)
            {
                newObject.transform.position = exitPoint.transform.position;
                newObject.SetActive(true);
                Rigidbody newObjectRigidbody = newObject.GetComponent<Rigidbody>();
                if (newObjectRigidbody != null)
                {
                    newObjectRigidbody.velocity = direction * projectileSpeed;
                    //newObjectRigidbody.velocity = new Vector3(direction.x, 0, direction.z) * projectileSpeed;
                    StartCoroutine(DisableObjectAfterTime(newObject, lifetime));
                }
            }
        }

        twinStickMovement.PlayerSpeed = 5;
        animator.speed = 1;
        animator.ResetTrigger("Attack");
    }

    private IEnumerator DisableObjectAfterTime(GameObject objectToDisable, float time)
    {
        yield return new WaitForSeconds(time);
        objectToDisable.SetActive(false);
    }

    private GameObject GetPooledObject()
    {
        for (int i = 0; i < objectPool.Count; i++)
        {
            if (!objectPool[i].activeInHierarchy)
            {
                return objectPool[i];
            }
        }

        if (objectPool.Count < maxObjects)
        {
            GameObject newObject = Instantiate(projectilePrefab, exitPoint.transform.position, Quaternion.identity);
            newObject.SetActive(false);
            objectPool.Add(newObject);
            return newObject;
        }
        return null;
    }

}

