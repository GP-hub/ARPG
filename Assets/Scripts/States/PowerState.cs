
using UnityEngine;

class PowerState : IState
{
    private Enemy enemy;

    void IState.Enter(Enemy enemy)
    {
        this.enemy = enemy;
        enemy.TargetPosition = enemy.Target.position;

        enemy.SetBoolSingle("TriggerPower");
        enemy.DecideNextPowerAbility();
        if (enemy.Agent.isOnNavMesh && enemy.Agent.enabled) enemy.Stop();

        //enemy.StartCoroutine(enemy.DelayedPowerEnter());
    }

    void IState.Exit()
    {
        if (enemy.Agent.isOnNavMesh && enemy.Agent.enabled) enemy.Agent.isStopped = false;

        enemy.ResetSingleBool("TriggerPower");

        enemy.ResetAttackingAndPowering();
    }
    void IState.Update()
    {
        if (enemy.IsAttacking || enemy.IsPowering) return;
        Utility.RotateTowardsTarget(enemy.transform, enemy.TargetPosition, enemy.RotationSpeed);
    }

    string IState.GetStateName()
    {
        return "PowerState";
    }

}
