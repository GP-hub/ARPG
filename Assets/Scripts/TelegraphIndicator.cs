using UnityEngine;
using UnityEngine.Rendering.Universal;

public class TelegraphIndicator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private DecalProjector staticDecal;
    [SerializeField] private DecalProjector growingDecal;

    [Header("Settings")]
    [SerializeField] private float growDuration;
    [SerializeField] private float AoESize;

    private float growSpeed; // How much to grow per second
    private float timer;
    private bool isActive;

    private void Awake()
    {
        staticDecal.gameObject.SetActive(false);
        growingDecal.gameObject.SetActive(false);
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
        staticDecal.fadeFactor = 0.35f;
        growingDecal.fadeFactor = 0.5f;

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

    public void SetIndicatorPosition(float growthDuration, float size)
    {
        growDuration = growthDuration;
        AoESize = size;
        ActivateTelegraph();
    }
}