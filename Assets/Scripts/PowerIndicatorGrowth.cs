using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PowerIndicatorGrowth : MonoBehaviour
{
    private Coroutine currentGrowthCoroutine;
    private DecalProjector decalProjector;

    [SerializeField] private float fadeOutDuration = 0.2f;

    private void Awake()
    {
        decalProjector = GetComponent<DecalProjector>();
        if (decalProjector == null)
        {
            Debug.LogError("No DecalProjector component found on the object.");
        }
    }
    public void StopGrowth()
    {
        if (currentGrowthCoroutine != null)
        {
            StopCoroutine(currentGrowthCoroutine);
            currentGrowthCoroutine = null;
            decalProjector.fadeFactor = 0f;
        }
    }

    public void StartGrowth(float duration)
    {
        if (currentGrowthCoroutine != null)
            StopCoroutine(currentGrowthCoroutine);

        currentGrowthCoroutine = StartCoroutine(GrowOverTime(duration));
    }

    private IEnumerator GrowOverTime(float duration)
    {
        Transform t = transform;
        t.localScale = Vector3.zero;

        if (decalProjector != null)
            decalProjector.fadeFactor = 1f;

        float timer = 0f;
        while (timer < duration)
        {
            float progress = timer / duration;
            t.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, progress);
            timer += Time.deltaTime;
            yield return null;
        }

        t.localScale = Vector3.one;

        // Fade out
        if (decalProjector != null)
        {
            float fadeTimer = 0f;
            while (fadeTimer < fadeOutDuration)
            {
                float fadeProgress = fadeTimer / fadeOutDuration;
                decalProjector.fadeFactor = Mathf.Lerp(1f, 0f, fadeProgress);
                fadeTimer += Time.deltaTime;
                yield return null;
            }
            decalProjector.fadeFactor = 0f;
        }
    }
}
