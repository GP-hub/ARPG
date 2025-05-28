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
    [SerializeField] private GameObject combustionStatusEffectVFX;

    private StatusEffectManager statusEffectManager;

    private float ultimateCooldownTimeElapsed;
    private Dash dashScript;
    private AttackAndPowerCasting attackAndPowerCastingScript;

    private int activeUltimateCount = 0;
    public bool IsUltimateActive => activeUltimateCount > 0;



    private bool isUltimateOnCooldown;
    private bool isCasting = false;

    private PlayerInput playerInput;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        dashScript = GetComponent<Dash>();
        attackAndPowerCastingScript = GetComponent<AttackAndPowerCasting>();
        statusEffectManager = GetComponent<StatusEffectManager>();
        playerInput.actions.FindAction("Ultimate").performed += OnUltimate;
        combustionStatusEffectVFX.SetActive(false);
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

        statusEffectManager?.ApplyOrRefreshEffect("Ultimate", ultimateDuration, Color.blue);

        StartCoroutine(CooldownUltimateCoroutine(ultimateCooldown));
        StartCoroutine(ModifyPlayerStatistics());
    }

    private IEnumerator ModifyPlayerStatistics()
    {
        activeUltimateCount++;
        if (activeUltimateCount == 1)
        {
            EventManager.Ultimate(true);
            combustionStatusEffectVFX.SetActive(true);
        }

        SpellCharge.BuffByUltimate();
        dashScript.BuffByUltimate();
        attackAndPowerCastingScript.BuffByUltimate();

        yield return new WaitForSeconds(ultimateDuration);

        SpellCharge.BuffByUltimate();
        dashScript.RemoveUltimateBuff();
        attackAndPowerCastingScript.RemoveUltimateBuff();

        activeUltimateCount--;
        if (activeUltimateCount == 0)
        {
            EventManager.Ultimate(false);
            combustionStatusEffectVFX.SetActive(false);
        }
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
