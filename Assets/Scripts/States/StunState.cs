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
        if (enemy.Agent.isOnNavMesh && enemy.Agent.enabled) enemy.Stop();
        enemy.SetBoolSingle("TriggerStun");
        enemy.ResetAttackingAndPowering();
    }

    void IState.Exit()
    {
        enemy.IsCC = false;
        if (enemy.Agent.isOnNavMesh && enemy.Agent.enabled) enemy.Agent.isStopped = false;
        enemy.ResetSingleBool("TriggerStun");
    }
    void IState.Update()
    {
        UpdateCountdown();
    }

    private void UpdateCountdown()
    {
        enemy.CCDuration -= Time.deltaTime;

        // If CCduration reaches zero or below, exit the state
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
