using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Combustion : MonoBehaviour
{
    //[SerializeField] private int ultimateSpeed;

    [SerializeField] private float ultimateDuration;
    [SerializeField] private float ultimateCooldown;
    [SerializeField] private Image ultimateCooldownImage;

    private float ultimateCooldownTimeElapsed;
    private Dash dashScript;
    private AttackAndPowerCasting attackAndPowerCastingScript;


    private bool isUltimateOnCooldown;
    private bool isCasting = false;

    private PlayerInput playerInput;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        dashScript = GetComponent<Dash>();
        attackAndPowerCastingScript = GetComponent<AttackAndPowerCasting>();
        playerInput.actions.FindAction("Ultimate").performed += OnUltimate;

        //fireballProcChance = this.transform.GetComponent<AttackAndPowerCasting>().fireballPrefab.GetComponent<Fireball>().currentProcChance;
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
            playerInput.actions.FindAction("Ultimate").performed -= OnUltimate;
        }
    }

    private void Casting(bool ultimate)
    {
        isCasting = ultimate;
    }

    private void OnUltimate(InputAction.CallbackContext context)
    {
        if (context.performed) StartUltimate();
    }

    private void StartUltimate()
    {
        if (isUltimateOnCooldown) return;
        if (isCasting) return;

        StartCoroutine(CooldownUltimateCoroutine(ultimateCooldown));
        StartCoroutine(ModifyPlayerStatistics());
    }

    private IEnumerator ModifyPlayerStatistics()
    {
        EventManager.Ultimate(true);


        SpellCharge.AddBonusProbability(100);
        dashScript.BuffByUltimate();
        attackAndPowerCastingScript.BuffByUltimate();

        yield return new WaitForSeconds(ultimateDuration);


        SpellCharge.RemoveBonusProbability(100);
        dashScript.RemoveUltimateBuff();
        attackAndPowerCastingScript.RemoveUltimateBuff();

        EventManager.Ultimate(false);
    }

    private IEnumerator CooldownUltimateCoroutine(float cd)
    {
        isUltimateOnCooldown = true;
        ultimateCooldownTimeElapsed = 0f;
        ultimateCooldownImage.fillAmount = 1;

        while (ultimateCooldownTimeElapsed < cd)
        {
            ultimateCooldownTimeElapsed += Time.deltaTime;
            ultimateCooldownImage.fillAmount = 1 - ultimateCooldownTimeElapsed / ultimateCooldown;
            yield return null;
        }
        ultimateCooldownImage.fillAmount = 0;
        isUltimateOnCooldown = false;
    }
}
