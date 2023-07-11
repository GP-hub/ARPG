using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements.Experimental;

public class Dash : MonoBehaviour
{
    [SerializeField] private int dashSpeed;
    [SerializeField] private float dashDuration;
    [SerializeField] private float dashCooldown;

    private bool isDashOnCooldown;
    private bool isCasting = false;

    private TwinStickMovement twinStickMovement;
    private PlayerInput playerInput;


    private void Awake()
    {
        twinStickMovement = GetComponent<TwinStickMovement>();
        playerInput = GetComponent<PlayerInput>();

        playerInput.actions.FindAction("Dash").performed += OnDash;
    }

    private void OnEnable()
    {
        EventManager.Instance.onCasting += Casting;
    }

    private void Casting(bool dashing)
    {
        isCasting = dashing;
    }

    private void OnDash(InputAction.CallbackContext context)
    {
        if (context.performed) StartDash();
    }

    private void StartDash()
    {
        if (isDashOnCooldown) return;
        if (isCasting) return;

        StartCoroutine(CooldownDashCoroutine(dashCooldown));
        StartCoroutine(ModifyPlayerMovementSpeed());
    }

    private IEnumerator ModifyPlayerMovementSpeed()
    {
        EventManager.Instance.Dashing(true);
        twinStickMovement.PlayerSpeed += dashSpeed;

        yield return new WaitForSeconds(dashDuration);

        twinStickMovement.PlayerSpeed -= dashSpeed;
        EventManager.Instance.Dashing(false);
    }

    private IEnumerator CooldownDashCoroutine(float cd)
    {
        isDashOnCooldown = true;
        float attackTimeElapsed = 0f;

        while (attackTimeElapsed < cd)
        {
            attackTimeElapsed += Time.deltaTime;
            yield return null;
        }

        isDashOnCooldown = false;
    }
}
