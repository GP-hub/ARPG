using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Dash : MonoBehaviour
{
    [SerializeField] private int dashSpeedPercent;
    [SerializeField] private float dashDuration;
    [SerializeField] private float dashCooldown;
    [SerializeField] private Image dashCooldownImage;

    private float dashCooldownTimeElapsed;


    private bool isDashOnCooldown;
    private bool isCasting = false;

    private PlayerMovement playerMovement;
    private PlayerInput playerInput;

    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        playerInput = GetComponent<PlayerInput>();

        playerInput.actions.FindAction("Dash").performed += OnDash;
    }

    private void OnEnable()
    {
        EventManager.onCasting += Casting;
    }

    private void OnDisable()
    {
        EventManager.onCasting -= Casting;

        if (playerInput != null)
        {
            playerInput.actions.FindAction("Dash").performed -= OnDash;
        }
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
        EventManager.Dashing(true);

        //playerMovement.CurrentPlayerSpeed += dashSpeed;
        playerMovement.AddSpeedModifier("Dash", dashSpeedPercent);

        yield return new WaitForSeconds(dashDuration);

        //playerMovement.CurrentPlayerSpeed -= dashSpeed;
        playerMovement.RemoveSpeedModifier("Dash");

        EventManager.Dashing(false);
    }

    private IEnumerator CooldownDashCoroutine(float cd)
    {
        isDashOnCooldown = true;
        dashCooldownTimeElapsed = 0f;
        dashCooldownImage.fillAmount = 1;

        while (dashCooldownTimeElapsed < cd)
        {
            dashCooldownTimeElapsed += Time.deltaTime;
            dashCooldownImage.fillAmount = 1 - dashCooldownTimeElapsed / dashCooldown;
            yield return null;
        }
        dashCooldownImage.fillAmount = 0;
        isDashOnCooldown = false;
    }
    public void IncreaseFireDashDuration(float bonusDuration)
    {
        dashDuration += bonusDuration;
    }

    public void DecreaseFireDashDuration(float bonusDuration)
    {
        dashDuration -= bonusDuration;
    }


}
