using UnityEngine;

class FollowState : IState
{
    private Enemy enemy;

    void IState.Enter(Enemy enemy)
    {
        this.enemy = enemy;
        enemy.SetBoolSingle("TriggerWalk");
        enemy.ResetAttackingAndPowering();
    }

    void IState.Exit()
    {
        enemy.ResetSingleBool("TriggerWalk");
    }

    void IState.Update()
    {
        //enemy.ResetAttackingAndPowering();
        // Continuously update the destination if the target moves
        if (enemy.Target != null)
        {
            enemy.MoveAIUnit(enemy.Target.position);
        }

        // Check if the enemy has reached its destination
        if (enemy.HasReachedDestination())
        {
            enemy.ChangeState(new IdleState());
        }
    }

    string IState.GetStateName()
    {
        return "FollowState";
    }
}
