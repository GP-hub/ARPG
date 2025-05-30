using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float playerSpeed;
    //private float currentPlayerSpeed;
    private Dictionary<string, float> speedModifiers = new();

    private float gravity = -9.81f;

    private bool isAttackCastHeldDown;
    private bool iPowerCastHeldDown;

    private Animator animator;
    private Vector2 movement;
    private Vector2 move;
    private Vector2 aim;
    private Vector3 playerVelocity;
    private Vector3 worldAim;
    private float smoothnessInputTransition = 2.5f; // 12.5f with old rotation method // 25f latest value

    private PlayerControls playerControls;
    private PlayerHealth playerHealth;

    private CharacterController controller;
    private NavMeshAgent navMeshAgent;

    private AttackAndPowerCasting attackAndPowerCasting;

    //public float CurrentPlayerSpeed { get => currentPlayerSpeed; set => currentPlayerSpeed = value; }


    //
    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        playerHealth = GetComponent<PlayerHealth>();
        attackAndPowerCasting = GetComponent<AttackAndPowerCasting>();
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        playerControls = new PlayerControls();
    }
    private void Start()
    {
        //currentPlayerSpeed = playerSpeed;
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }


    // 
    void Update()
    {
        if (!playerHealth.IsAlive) return;

        //playerSpeed = Mathf.Max(0f, playerSpeed);
        //currentPlayerSpeed = Mathf.Clamp(currentPlayerSpeed, 0f, playerSpeed);

        HandleInput();
        HandleRotation();

        HandleAnimation();
        HandleMovement();

        isAttackCastHeldDown = playerControls.Controls.Attack.ReadValue<float>() > .1;
        iPowerCastHeldDown = playerControls.Controls.Power.ReadValue<float>() > .1;
    }

    private void FixedUpdate()
    {
    }

    private void HandleAnimation()
    {
        Vector3 moveDirection = new Vector3(movement.x, 0, movement.y);

        if (moveDirection.magnitude > 0.01f && currentPlayerSpeed != 0)
        {

            float angle = Vector3.SignedAngle(transform.forward, moveDirection.normalized, Vector3.up);

            float targetInputX = Mathf.Sin(angle * Mathf.Deg2Rad);
            float targetInputY = Mathf.Cos(angle * Mathf.Deg2Rad);

            animator.SetFloat("InputX", Mathf.Lerp(animator.GetFloat("InputX"), targetInputX, Time.fixedDeltaTime * smoothnessInputTransition));
            animator.SetFloat("InputY", Mathf.Lerp(animator.GetFloat("InputY"), targetInputY, Time.fixedDeltaTime * smoothnessInputTransition));

        }
        else
        {
            //animator.SetFloat("InputX", 0);
            //animator.SetFloat("InputY", 0);

            animator.SetFloat("InputX", Mathf.Lerp(animator.GetFloat("InputX"), 0, Time.fixedDeltaTime * smoothnessInputTransition));
            animator.SetFloat("InputY", Mathf.Lerp(animator.GetFloat("InputY"), 0, Time.fixedDeltaTime * smoothnessInputTransition));

        }

        if (attackAndPowerCasting.IsCasting)
        {
            HandleAnimationWeight(1, "Aiming");
        }
        else
        {
            HandleAnimationWeight(0, "Aiming");
        }
        animator.SetFloat("MoveSpeed", currentPlayerSpeed / playerSpeed);

    }

    //public void IncreasePlayerMaxMovespeed(int speedBuff)
    //{
    //    playerSpeed += speedBuff;
    //}

    public float currentPlayerSpeed
    {
        get
        {
            float totalModifier = 0f;
            foreach (float modifier in speedModifiers.Values)
            {
                totalModifier += modifier;
            }

            float finalSpeed = playerSpeed * (1f + totalModifier / 100f);
            return Mathf.Max(0f, finalSpeed); // Prevent negative speed
        }
    }


    public void AddSpeedModifier(string source, float multiplier)
    {
        if (!speedModifiers.ContainsKey(source))
            speedModifiers.Add(source, multiplier);
    }

    public void RemoveSpeedModifier(string source)
    {
        if (speedModifiers.ContainsKey(source))
            speedModifiers.Remove(source);
    }

    public void SetSpeedModifier(string source, float multiplier)
    {
        speedModifiers[source] = multiplier; // Add or overwrite
    }


    private void HandleAnimationWeight(float targetWeight, string layerName)
    {
        // Get the current weight
        float currentWeight = animator.GetLayerWeight(animator.GetLayerIndex(layerName));

        // Smoothly interpolate between current and target weight over 0.3 seconds
        float newWeight = Mathf.Lerp(currentWeight, targetWeight, Time.deltaTime / .2f);

        // Set the new weight value
        animator.SetLayerWeight(animator.GetLayerIndex(layerName), newWeight);
    }

    private void HandleInput()
    {
        movement = playerControls.Controls.Movements.ReadValue<Vector2>();
        aim = playerControls.Controls.Aim.ReadValue<Vector2>();
    }

    public void RecoverMovementSpeed()
    {
        //CurrentPlayerSpeed = PlayerSpeed;
        //StartCoroutine(RestoreSpeedCoroutine(.2f, playerSpeed, currentPlayerSpeed));
    }

    //public IEnumerator RestoreSpeedCoroutine(float duration, float targetSpeed, float currentSpeed)
    //{
    //    float startSpeed = currentSpeed;
    //    float elapsed = 0f;

    //    while (elapsed < duration)
    //    {
    //        elapsed += Time.deltaTime;
    //        currentPlayerSpeed = Mathf.Lerp(startSpeed, targetSpeed, elapsed / duration);
    //        yield return null;
    //    }

    //    currentPlayerSpeed = targetSpeed;
    //}




    private void HandleMovement()
    {
        Vector3 move = new Vector3(movement.x, 0, movement.y);
        controller.Move(move * Time.deltaTime * currentPlayerSpeed);

        if (!controller.isGrounded)
        {
            playerVelocity.y += gravity * Time.deltaTime;
            controller.Move(playerVelocity * Time.deltaTime);
        }
    }

    // Constantly rotation toward cursor
    private void HandleRotation()
    {
        Ray ray = Camera.main.ScreenPointToRay(aim);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        float rayDistance;

        if (groundPlane.Raycast(ray, out rayDistance))
        {
            Vector3 point = ray.GetPoint(rayDistance);
            LookAt(point);
            worldAim = point;
        }
    }

    private void LookAt(Vector3 lookPoint)
    {
        Vector3 heightCorrectedPoint = new Vector3(lookPoint.x, transform.position.y, lookPoint.z);
        Quaternion targetRotation = Quaternion.LookRotation(heightCorrectedPoint - transform.position);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 50 /2);
    }


    private void HandleNavMeshAgentObstacle()
    {
        Vector3 moveDirection = new Vector3(movement.x, 0, movement.y);

        if (moveDirection.magnitude > 0.01f)
        {

        }
        else
        {

        }
    }

    private void TriggerAnimationOnDeath()
    {
        controller.enabled = false;
        navMeshAgent.enabled = false;
    }
}
