using System;

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
        if (onEnemyDecideNextMove != null)
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

    public static event Action<int, int> onPlayerUpdateHealthUI;
    public static void PlayerUpdateHealthUI(int currentLife, int maxLife)
    {
        if (onPlayerUpdateHealthUI != null)
        {
            onPlayerUpdateHealthUI(currentLife, maxLife);
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

    public static event Action onPlayerDeath;
    public static void PlayerDeath()
    {
        if (onPlayerDeath != null)
        {
            onPlayerDeath();
        }
    }

    public static event Action onBossDeath;
    public static void BossDeath()
    {
        if (onBossDeath != null)
        {
            onBossDeath();
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

    public static event Action onGetUnits;
    public static void GetUnits()
    {
        if (onGetUnits != null)
        {
            onGetUnits();
        }
    }

    public static event Action<int> onBossRockFall;
    public static void BossRockFall(int numberOfRocks)
    {
        if (onBossRockFall != null)
        {
            onBossRockFall(numberOfRocks);
        }
    }

    public static event Action onBossHitWall;
    public static void BossHitWall()
    {
        if (onBossHitWall != null)
        {
            onBossHitWall();
        }
    }

    public static event Action<string, int> onMessageEvent;
    public static void MessageEvent(string message, int timerDisable)
    {
        if (onMessageEvent != null)
        {
            onMessageEvent(message, timerDisable);
        }
    }


}
