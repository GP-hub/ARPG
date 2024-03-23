using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Combustion : MonoBehaviour
{
    [SerializeField] private int ultimateSpeed;

    [SerializeField] private float ultimateDuration;
    [SerializeField] private float ultimateCooldown;
    [SerializeField] private Image ultimateCooldownImage;

    private float ultimateCooldownTimeElapsed;

    private int fireballProcChance;


    private bool isUltimateOnCooldown;
    private bool isCasting = false;

    private PlayerMovement twinStickMovement;
    private PlayerInput playerInput;

    private void Awake()
    {
        twinStickMovement = GetComponent<PlayerMovement>();
        playerInput = GetComponent<PlayerInput>();

        playerInput.actions.FindAction("Ultimate").performed += OnUltimate;

        //fireballProcChance = this.transform.GetComponent<AttackAndPowerCasting>().fireballPrefab.GetComponent<Fireball>().currentProcChance;
    }


    private void OnEnable()
    {
        EventManager.onCasting += Casting;
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

        PlayerStats.AddBonusProbability(100);

        // Modification to player stats
        yield return new WaitForSeconds(ultimateDuration);

        PlayerStats.RemoveBonusProbability(100);

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
