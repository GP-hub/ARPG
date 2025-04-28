using System.Collections;
using UnityEngine;

public class RockMaterialFade : MonoBehaviour
{
    [SerializeField] private Renderer rockRenderer;
    private Material fadeMaterial;
    private Color originalColor;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake ()
    {
        if (rockRenderer == null)
            rockRenderer = GetComponentInChildren<Renderer>();

        fadeMaterial = rockRenderer.material;
    }

    private void OnEnable()
    {
        ResetFade();
    }

    private void OnDisable()
    {
        ResetFade();
    }

    private IEnumerator RockFalling()
    {

        originalColor = fadeMaterial.color;
        float fadeDuration = 1f; // Duration for fading (can be adjusted)
        float fadeElapsed = 0f;

        // Gradually reduce the alpha to 0
        while (fadeElapsed < fadeDuration)
        {
            fadeElapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, fadeElapsed / fadeDuration);

            // Set the new color with reduced alpha
            fadeMaterial.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);

            yield return null;
        }

        // Final state with fully transparent material
        fadeMaterial.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);

        // Disable the rock object after fading
        this.gameObject.SetActive(false);
    }

    private void ResetFade()
    {
        // Set the initial color to fully opaque
        originalColor = fadeMaterial.color;
        fadeMaterial.color = new Color(originalColor.r, originalColor.g, originalColor.b, 1f);
    }
}
