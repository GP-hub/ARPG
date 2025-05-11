using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBarUI : MonoBehaviour
{

    [SerializeField] private Image fillImage;
    [SerializeField] private TextMeshProUGUI healthText;

    void OnEnable()
    {
        EventManager.onPlayerUpdateHealthUI += UpdateVisuals;
    }

    void OnDisable()
    {
        EventManager.onPlayerUpdateHealthUI -= UpdateVisuals;
    }

    private void UpdateVisuals(int current, int max)
    {
        if (fillImage != null && max > 0)
        {
            // Cast to float so we don't get integer division!
            fillImage.fillAmount = Mathf.Clamp01((float)current / max);
        }

        if (healthText != null)
        {
            healthText.text = $"{current} / {max}";
        }
    }
}
