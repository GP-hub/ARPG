
using UnityEngine;

class PowerState : IState
{
    private Enemy enemy;

    void IState.Enter(Enemy enemy)
    {
        this.enemy = enemy;

        if (enemy.Agent.isOnNavMesh && enemy.Agent.enabled) enemy.Stop();

        enemy.SetBoolSingle("TriggerPower");

    }

    void IState.Exit()
    {
        if (enemy.Agent.isOnNavMesh && enemy.Agent.enabled) enemy.Agent.isStopped = false;

        //enemy.Animator.SetFloat("AttackAndPower", 0f);

        enemy.ResetSingleBool("TriggerPower");

        enemy.ResetAttackingAndPowering();
    }
    void IState.Update()
    {
        if (enemy.isCharging) return;

        enemy.transform.LookAt(enemy.Target);
    }

    string IState.GetStateName()
    {
        return "PowerState";
    }

}
