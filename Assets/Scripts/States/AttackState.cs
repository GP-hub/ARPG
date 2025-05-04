
using UnityEngine;

class AttackState : IState
{
    private Enemy enemy;

    void IState.Enter(Enemy enemy)
    {
        this.enemy = enemy;
        enemy.TargetPosition = enemy.Target.position;
        enemy.DecideNextAbility();
        enemy.SetBoolSingle("TriggerAttack");
        if (enemy.Agent.isOnNavMesh && enemy.Agent.enabled) enemy.Stop();
        enemy.Animator.SetFloat("AttackTree", enemy.BlendTreeThreshold());

        //enemy.StartCoroutine(enemy.DelayedAttackEnter());
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
        if (enemy.isCharging) return;
        Utility.RotateTowardsTarget(enemy.transform, enemy.TargetPosition, enemy.RotationSpeed);

        //Utility.RotateTowardsTarget(enemy.transform, enemy.Target, enemy.RotationSpeed);
    }

    string IState.GetStateName()
    {
        return "AttackState";
    }

}
