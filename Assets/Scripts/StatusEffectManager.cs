using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StatusEffectManager : MonoBehaviour
{
    public class StatusEffect
    {
        public string Name;
        public Color DisplayColor;
        public float Duration;
        public float TimeRemaining;

        public StatusEffect(string name, float duration, Color color)
        {
            Name = name;
            Duration = duration;
            TimeRemaining = duration;
            DisplayColor = color;
        }
    }

    private readonly List<StatusEffect> activeEffects = new();
    public event Action<StatusEffect> OnDisplayEffectChanged;

    private StatusEffect currentDisplayedEffect;

    void Update()
    {
        if (activeEffects.Count == 0) return;

        bool removed = false;
        foreach (StatusEffect effect in activeEffects.ToList())
        {
            effect.TimeRemaining -= Time.deltaTime;
            if (effect.TimeRemaining <= 0)
            {
                activeEffects.Remove(effect);
                removed = true;
            }
        }

        if (removed)
            UpdateDisplayEffect();
    }

    public void ApplyOrRefreshEffect(string name, float duration, Color color)
    {
        StatusEffect existing = activeEffects.FirstOrDefault(e => e.Name == name);
        if (existing != null)
        {
            existing.Duration = duration;
            existing.TimeRemaining = duration;
        }
        else
        {
            activeEffects.Add(new StatusEffect(name, duration, color));
        }

        UpdateDisplayEffect();
    }

    public void RemoveEffect(string name)
    {
        bool removed = activeEffects.RemoveAll(e => e.Name == name) > 0;
        if (removed)
            UpdateDisplayEffect();
    }

    private void UpdateDisplayEffect()
    {
        StatusEffect shortest = activeEffects.OrderBy(e => e.TimeRemaining).FirstOrDefault();

        if (shortest != currentDisplayedEffect)
        {
            currentDisplayedEffect = shortest;
            OnDisplayEffectChanged?.Invoke(currentDisplayedEffect);
        }
    }

    public float GetRemainingTime(string name)
    {
        return activeEffects.FirstOrDefault(e => e.Name == name)?.TimeRemaining ?? 0f;
    }
}
