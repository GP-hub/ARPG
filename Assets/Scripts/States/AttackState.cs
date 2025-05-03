
using UnityEngine;

class AttackState : IState
{
    private Enemy enemy;

    void IState.Enter(Enemy enemy)
    {
        this.enemy = enemy;
        enemy.StartCoroutine(enemy.DelayedAttackEnter());
    }

    void IState.Exit()
    {
        if (enemy.Agent.isOnNavMesh && enemy.Agent.enabled) enemy.Agent.isStopped = false;

        enemy.ResetSingleBool("TriggerAttack");

        enemy.ResetAttackingAndPowering();
    }
    void IState.Update()
    {
        if (enemy.Target == null)
        {
            enemy.ChangeState(new IdleState());
        }
        if (enemy.IsAttacking || enemy.IsPowering) return;

        Utility.RotateTowardsTarget(enemy.transform, enemy.Target, enemy.RotationSpeed);
    }

    string IState.GetStateName()
    {
        return "AttackState";
    }

}
