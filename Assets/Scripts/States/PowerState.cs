
using UnityEngine;

class PowerState : IState
{
    private Enemy enemy;

    void IState.Enter(Enemy enemy)
    {
        this.enemy = enemy;
        enemy.TargetPosition = enemy.Target.position;

        enemy.DecideNextPowerAbility();
        enemy.SetBoolSingle("TriggerPower");
        enemy.TargetPosition = enemy.GetInaccurateTarget(enemy.Target.position);
        if (enemy.Agent.isOnNavMesh && enemy.Agent.enabled) enemy.Stop();
        enemy.Animator.SetFloat("PowerTree", enemy.GetCurrentPowerAbilityIndex());
        enemy.ResetPerformedBehavior();
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
