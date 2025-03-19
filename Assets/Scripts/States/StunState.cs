using UnityEngine;

public class StunState : IState
{
    private Enemy enemy;

    private float countdownDuration;
    private float countdownTimer;

    void IState.Enter(Enemy enemy)
    {
        this.enemy = enemy;
        enemy.IsCC = true;
        enemy.SetBoolSingle("TriggerStun");
    }

    void IState.Exit()
    {
        enemy.IsCC = false;
        enemy.ResetSingleBool("TriggerStun");
    }
    void IState.Update()
    {
        UpdateCountdown();
    }

    private void UpdateCountdown()
    {
        // If countdownTimer reaches zero or below, exit the state
        if (enemy.CCDuration <= 0)
        {
            enemy.ChangeState(new IdleState());
        }
    }

    string IState.GetStateName()
    {
        return "StunState";
    }
}
