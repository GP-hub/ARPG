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

    public event Action onEnemyDecideNextMove;
    public void EnemyDecideNextMove()
    {
        if (onEnemyDecideNextMove !=null)
        {
            onEnemyDecideNextMove();
        }
    }

    public event Action<int> onPlayerTakeDamage;
    public void PlayerTakeDamage(int damage)
    {
        if (onPlayerTakeDamage != null)
        {
            onPlayerTakeDamage(damage);
        }
    }

    public event Action<int> onPlayerTakeHeal;
    public void PlayerTakeHeal(int heal)
    {
        if (onPlayerTakeHeal != null)
        {
            onPlayerTakeHeal(heal);
        }
    }

    public event Action<Enemy, string> onEnemyTakeDamage;
    public void EnemyTakeDamage(Enemy enemy, string skill)
    {
        if (onEnemyTakeDamage != null)
        {
            onEnemyTakeDamage(enemy, skill);
        }
    }

    public event Action<Enemy, string> onEnemyGetCC;
    public void EnemyGetCC(Enemy enemy, string skill)
    {
        if (onEnemyGetCC != null)
        {
            onEnemyGetCC(enemy, skill);
        }
    }

    public event Action<int> onEnemyDeath;
    public void EnemyDeath(int xp)
    {
        if (onEnemyDeath != null)
        {
            onEnemyDeath(xp);
        }
    }



}
