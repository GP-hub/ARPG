using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatusEffectUI : MonoBehaviour
{
   [SerializeField] private TextMeshProUGUI effectNameText;
   [SerializeField] private Image progressBar;
   [SerializeField] private Image progressBarBackground;
    private StatusEffectManager effectManager;
    private StatusEffectManager.StatusEffect currentEffect;

    public void Initialize(StatusEffectManager manager)
    {
        effectManager = manager;
        effectManager.OnDisplayEffectChanged += OnEffectChanged;

        SetAlpha(0); // Hide on init
        progressBar.fillAmount = 0;
    }

    private void OnEffectChanged(StatusEffectManager.StatusEffect effect)
    {
        currentEffect = effect;

        if (effect == null)
        {
            SetAlpha(0); // Hide
            progressBar.fillAmount = 0;
            return;
        }

        effectNameText.enabled = true;
        effectNameText.text = effect.Name;
        effectNameText.color = effect.DisplayColor;

        progressBar.color = effect.DisplayColor;
        progressBarBackground.color = Color.black;
    }

    void Update()
    {
        if (currentEffect != null)
        {
            float fill = Mathf.Clamp01(currentEffect.TimeRemaining / currentEffect.Duration);
            progressBar.fillAmount = fill;
        }
    }

    private void SetAlpha(float alpha)
    {
        Color textColor = effectNameText.color;
        effectNameText.color = new Color(textColor.r, textColor.g, textColor.b, alpha);

        Color barColor = progressBar.color;
        progressBar.color = new Color(barColor.r, barColor.g, barColor.b, alpha);

        Color barBackgroundColor = progressBar.color;
        progressBarBackground.color = new Color(barColor.r, barColor.g, barColor.b, alpha);
    }
}
