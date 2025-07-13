using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UIElements;

public class TelegraphConeIndicator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private DecalProjector staticDecal;
    [SerializeField] private DecalProjector growingDecal;

    private Material staticDecalMaterial;
    private Material growingDecalMaterial;

    [Header("Settings")]
    [SerializeField] private float growDuration;
    [SerializeField] private float AoESize;
    //[SerializeField] private float fillAmountDegrees;

    private float growSpeed; // How much to grow per second
    private float timer;
    private bool isActive;

    private void Awake()
    {
        staticDecal.gameObject.SetActive(false);
        growingDecal.gameObject.SetActive(false);

        staticDecalMaterial = staticDecal.material;
        growingDecalMaterial = growingDecal.material;

        staticDecal.fadeFactor = 0;
        growingDecal.fadeFactor = 0;
    }

    private void OnEnable()
    {

    }

    private void OnDisable()
    {
        ResetIndicator();
    }

    public void ActivateTelegraph()
    {
        staticDecal.fadeFactor = .75f;
        growingDecal.fadeFactor = .5f;

        staticDecal.size = new Vector3(AoESize, AoESize, AoESize);
        growingDecal.size = Vector3.zero;

        staticDecal.gameObject.SetActive(true);
        growingDecal.gameObject.SetActive(true);

        growSpeed = AoESize / growDuration; // We assume uniform size (X = Z for circle)
        timer = 0f;
        isActive = true;
    }

    private void Update()
    {
        if (!isActive) return;

        timer += Time.deltaTime;

        // Grow uniformly per second
        float currentSize = growSpeed * timer;
        currentSize = Mathf.Min(currentSize, AoESize); // Clamp if overshoot

        growingDecal.size = new Vector3(currentSize, currentSize, currentSize);

        if (currentSize >= AoESize)
        {
            ResetIndicator();
        }
    }

    private void ResetIndicator()
    {
        growingDecal.gameObject.SetActive(false);
        staticDecal.gameObject.SetActive(false);
        this.gameObject.SetActive(false); // Disable parent object too
        isActive = false;
    }

    public void SetIndicatorPosition(float growthDuration, float size, float fillAmountDegrees, Transform transform)
    {
        float fillAmount = Mathf.Clamp01(fillAmountDegrees / 360f);
        staticDecalMaterial.SetFloat("_FillAmount", fillAmount);
        growingDecalMaterial.SetFloat("_FillAmount", fillAmount);

        staticDecal.transform.localRotation = Quaternion.Euler(90, fillAmountDegrees / 2, 0);
        growingDecal.transform.localRotation = Quaternion.Euler(90, fillAmountDegrees / 2, 0);

        SetDirection(transform);

        growDuration = growthDuration;
        AoESize = size;
        ActivateTelegraph();
    }

    private void SetDirection(Transform sourceTransform)
    {
        // Get the forward direction of the source, flatten to horizontal
        Vector3 forward = sourceTransform.forward;
        forward.y = 0f;
        if (forward != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(forward);
            // Rotate THIS indicator to match the source's horizontal direction
            transform.rotation = Quaternion.Euler(0, lookRotation.eulerAngles.y, 0);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1f, 0f, 0f, 0.75f); // semi-transparent red

        // Default radius for editor (when not in play mode)
        float radius = AoESize > 0 ? AoESize : 1f;

        Gizmos.DrawSphere(transform.position, radius/2);
    }
}
