using UnityEngine;
using System;
using Unity.VisualScripting;

public static class EventManager/* : Singleton<EventManager>*/
{
    public static int character;

    public static event Action onEventEnded; 
    public static void EventEnded()
    {
        if (onEventEnded != null)
        {
            onEventEnded();
        }
    }


    public static event Action<bool> onDashing;
    public static void Dashing(bool dashing)
    {
        if (onDashing != null)
        {
            onDashing(dashing);
        }
    }

    public static event Action<bool> onUltimate;
    public static void Ultimate(bool ultimate)
    {
        if (onUltimate != null)
        {
            onUltimate(ultimate);
        }
    }

    public static event Action<bool> onCasting;
    public static void Casting(bool dashing)
    {
        if (onCasting != null)
        {
            onCasting(dashing);
        }
    }

    public static event Action<int> onFireChargeCountChange;
    public static void FireChargeCountChange(int chargeNbr)
    {
        if (onFireChargeCountChange != null)
        {
            onFireChargeCountChange(chargeNbr);
        }
    }

    public static event Action onEnemyDecideNextMove;
    public static void EnemyDecideNextMove()
    {
        if (onEnemyDecideNextMove !=null)
        {
            onEnemyDecideNextMove();
        }
    }

    public static event Action<int> onPlayerTakeDamage;
    public static void PlayerTakeDamage(int damage)
    {
        if (onPlayerTakeDamage != null)
        {
            onPlayerTakeDamage(damage);
        }
    }

    public static event Action<int> onPlayerTakeHeal;
    public static void PlayerTakeHeal(int heal)
    {
        if (onPlayerTakeHeal != null)
        {
            onPlayerTakeHeal(heal);
        }
    }

    public static event Action<Enemy, string> onEnemyTakeDamage;
    public static void EnemyTakeDamage(Enemy enemy, string skill)
    {
        if (onEnemyTakeDamage != null)
        {
            onEnemyTakeDamage(enemy, skill);
        }
    }

    public static event Action<Enemy, string> onEnemyGetCC;
    public static void EnemyGetCC(Enemy enemy, string skill)
    {
        if (onEnemyGetCC != null)
        {
            onEnemyGetCC(enemy, skill);
        }
    }

    public static event Action<int> onEnemyDeath;
    public static void EnemyDeath(int xp)
    {
        if (onEnemyDeath != null)
        {
            onEnemyDeath(xp);
        }
    }

    public static event Action<string> onSceneLoad;
    public static void SceneLoad(string sceneName)
    {
        if (onSceneLoad != null)
        {
            onSceneLoad(sceneName);
        }
    }



}
