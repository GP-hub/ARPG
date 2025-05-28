using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Dash : MonoBehaviour
{
    [SerializeField] private int dashSpeedPercent;
    [SerializeField] private float dashDuration;
    [SerializeField] private float dashCooldown;
    [SerializeField] private float bonusDashPercentDurationFromUltimate;
    [SerializeField] private Image dashCooldownImage;

    private float dashCooldownTimeElapsed;

    private int ultimateBuffCount = 0;
    public bool isBuffedUltimate => ultimateBuffCount > 0;


    private bool isDashOnCooldown;
    private bool isCasting = false;

    private PlayerMovement playerMovement;
    private PlayerInput playerInput;
    private StatusEffectManager statusEffectManager;

    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        playerInput = GetComponent<PlayerInput>();
        statusEffectManager = GetComponent<StatusEffectManager>();
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

        float currentDashDuration = dashDuration;

        if (isBuffedUltimate) currentDashDuration = dashDuration * (1f + bonusDashPercentDurationFromUltimate / 100f);

        statusEffectManager?.ApplyOrRefreshEffect("Firedash", currentDashDuration, Color.red);

        yield return new WaitForSeconds(currentDashDuration);

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

    public void BuffByUltimate()
    {
        ultimateBuffCount++;
    }
    public void RemoveUltimateBuff()
    {
        ultimateBuffCount = Mathf.Max(0, ultimateBuffCount - 1);
    }

}
