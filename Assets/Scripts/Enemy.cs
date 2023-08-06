using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float health, maxHealth = 30;
    [SerializeField] private float attackRange;
    private LayerMask groundLayerMask;

    [Space(10)]
    [Header("Healthbar")]
    [SerializeField] private GameObject healthBarPrefab;
    [SerializeField] private RectTransform healthPanelRect;

    [Space(10)]
    [Header("Ranged Attack")]
    [SerializeField] private GameObject exitPoint;
    [SerializeField] private GameObject fireballPrefab;
    [SerializeField] private GameObject fireballExplosionPrefab;
    [SerializeField] private float attackProjectileSpeed;
    private int maxObjectsForPooling = 5;


    private List<GameObject> fireballObjectPool = new List<GameObject>();

    private Animator animator;
    private Rigidbody rb;

    private Healthbar healthBar;

    private Transform target;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

        groundLayerMask = LayerMask.GetMask("Ground");
    }

    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
        GeneratePlayerHealthBar(this.gameObject);

        // Pass the player as the target for now
        target = GameObject.Find("Player").transform;

        PoolingFireballObject();
    }


    private void LateUpdate()
    {
        HandleAttack();
        HandleAnimation();
        //HandleNavMeshAgentObstacle();
    }

    public void TakeDamage(float damageAmount)
    {
        health -= damageAmount;

        if (health <= 0)
        {
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
            Move();
            HandleAttackAnimation(false);
            return;
        }


        if (CanSeePlayer())
        {
            if (distanceToTarget <= attackRange)
            {
                Stop();
                HandleAttackAnimation(true);
            }
            if (distanceToTarget > attackRange)
            {
                Move();
                HandleAttackAnimation(false);
            }
        }
    }

    void HandleAttackAnimation(bool onRange)
    {
        if (onRange)
        {
            animator.SetBool("IsAttacking", true);

            LookTowards();

            animator.SetBool("IsWalking", false);
            animator.SetBool("IsIdle", false);

        }
        if (!onRange)
        {
            animator.SetBool("IsAttacking", false);
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

    public void MeleeHit()
    {
        // Range of the sphere of the melee hit
        Collider[] colliders = Physics.OverlapSphere(transform.position, 5f);

        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Player"))
            {
                // Damage player here
            }
        }
    }

    public void RangedHit()
    {
        Vector3 targetCorrectedPosition = target.transform.position;
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
            }
        }
    }

    void HandleAnimation()
    {
        if (animator.GetBool("IsAttacking")) return;

        //float currentSpeed = rb.velocity.magnitude;

        //if (currentSpeed > 0.1f)
        //{
        //    animator.SetBool("IsWalking", true);
        //    animator.SetBool("IsIdle", false);
        //}
        //else
        //{
        //    animator.SetBool("IsIdle", true);
        //    animator.SetBool("IsWalking", false);
        //}
    }


    void Move()
    {
    }

    void Stop()
    {
    }

    private void GeneratePlayerHealthBar(GameObject player)
    {
        GameObject healthBarGo = Instantiate(healthBarPrefab);
        healthBar = healthBarGo.GetComponent<Healthbar>();
        healthBar.SetHealthBarData(player.transform, healthPanelRect);
        healthBar.transform.SetParent(healthPanelRect, false);
    }

    Vector3 AdjustTargetPosition(Transform transform)
    {
        Vector3 vectorAB = transform.position - this.transform.position;

        Vector3 direction = vectorAB.normalized;

        // Small offset to solve enemy not being close enough not matter the stopping distance .5f
        Vector3 targetPosition = transform.position + direction * .5f;

        return targetPosition;
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
}
