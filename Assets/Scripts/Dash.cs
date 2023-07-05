using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Dash : MonoBehaviour
{
    [SerializeField] private int dashSpeed;
    [SerializeField] private float dashDuration;

    private bool isDashing // FIX THE COROUTINE

    private TwinStickMovement twinStickMovement;
    private PlayerInput playerInput;

    private void Awake()
    {
        twinStickMovement = GetComponent<TwinStickMovement>();
        playerInput = GetComponent<PlayerInput>();

        playerInput.actions.FindAction("Dash").performed += OnDash;
    }

    private void OnDash(InputAction.CallbackContext context)
    {
        if (context.performed) StartDash();
    }

    private void StartDash()
    {
        StartCoroutine(ModifyPlayerMovementSpeed());
    }

    private IEnumerator ModifyPlayerMovementSpeed()
    {
        twinStickMovement.PlayerSpeed = dashSpeed;

        yield return new WaitForSeconds(dashDuration);

        twinStickMovement.PlayerSpeed = 5;
    }

    private IEnumerator CooldownDashCoroutine(float cd)
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
}
