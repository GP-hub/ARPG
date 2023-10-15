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


    private bool isUltimateOnCooldown;
    private bool isCasting = false;

    private TwinStickMovement twinStickMovement;
    private PlayerInput playerInput;

    private void Awake()
    {
        twinStickMovement = GetComponent<TwinStickMovement>();
        playerInput = GetComponent<PlayerInput>();

        playerInput.actions.FindAction("Ultimate").performed += OnUltimate;
    }

    private void OnEnable()
    {
        EventManager.Instance.onCasting += Casting;
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
        EventManager.Instance.Ultimate(true);

        // Modification to player stats

        yield return new WaitForSeconds(ultimateDuration);

        // Back to basic stats

        EventManager.Instance.Ultimate(false);
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
