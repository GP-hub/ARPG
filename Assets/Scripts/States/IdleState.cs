using UnityEngine;

class IdleState : IState
{
    private Enemy enemy;

    void IState.Enter(Enemy enemy)
    {
        this.enemy = enemy;
        if (enemy.Agent.isOnNavMesh && enemy.Agent.enabled) enemy.Stop();
        enemy.SetBoolSingle("TriggerIdle");
        enemy.ResetAttackingAndPowering();

    }

    void IState.Exit()
    {
        if (enemy.Agent.isOnNavMesh && enemy.Agent.enabled) enemy.Agent.isStopped = false;
        enemy.ResetSingleBool("TriggerIdle");
    }

    void IState.Update()
    {
        //enemy.ResetAttackingAndPowering();
        if (enemy.Target != null && enemy.CanSeeTarget(enemy.Target))
        {
            Utility.RotateTowardsTarget(enemy.transform, enemy.Target.position, enemy.RotationSpeed);
            //enemy.ChangeState(new FollowState());
        }
    }

    string IState.GetStateName()
    {
        return "IdleState";
    }

}
