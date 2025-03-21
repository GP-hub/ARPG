using UnityEngine;

class IdleState : IState
{
    private Enemy enemy;

    void IState.Enter(Enemy enemy)
    {
        this.enemy = enemy;
        this.enemy.Target = null;
        enemy.SetBoolSingle("TriggerIdle");
        enemy.ResetAttackingAndPowering();
    }

    void IState.Exit()
    {
        enemy.ResetSingleBool("TriggerIdle");
    }

    void IState.Update()
    {
        //enemy.ResetAttackingAndPowering();
        if (enemy.Target != null)
        {
            enemy.ChangeState(new FollowState());
        }
    }

    string IState.GetStateName()
    {
        return "IdleState";
    }

}
