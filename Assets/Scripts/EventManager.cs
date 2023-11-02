using UnityEngine;
using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EventManager : Singleton<EventManager>
{
    // GENERAL CONSOLE //
    public event Action onEventEnded; // trigger console display plate back to passive/neutral image when event have ended
    public void EventEnded()
    {
        if (onEventEnded != null)
        {
            onEventEnded();
        }
    }

    public event Action onTriggerPressTriggerButton;
    public void TriggerPressTriggerButton()
    {
        if (onTriggerPressTriggerButton != null)
        {
            onTriggerPressTriggerButton();
        }
    }

    public event Action<bool> onDashing;
    public void Dashing(bool dashing)
    {
        if (onDashing != null)
        {
            onDashing(dashing);
        }
    }

    public event Action<bool> onUltimate;
    public void Ultimate(bool ultimate)
    {
        if (onUltimate != null)
        {
            onUltimate(ultimate);
        }
    }

    public event Action<bool> onCasting;
    public void Casting(bool dashing)
    {
        if (onCasting != null)
        {
            onCasting(dashing);
        }
    }

    public event Action<int> onFireChargeCountChange;
    public void FireChargeCountChange(int chargeNbr)
    {
        if (onFireChargeCountChange != null)
        {
            onFireChargeCountChange(chargeNbr);
        }
    }

}
