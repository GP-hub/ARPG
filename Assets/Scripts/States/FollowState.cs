using UnityEngine;

class FollowState : IState
{
    private Enemy enemy;

    void IState.Enter(Enemy enemy)
    {
        this.enemy = enemy;
        enemy.SetTriggerSingle("TriggerWalk");
        enemy.ResetAttackingAndPowering();
    }

    void IState.Exit()
    {
        enemy.ResetTriggerSingle("TriggerWalk");
    }

    void IState.Update()
    {
        enemy.ResetAttackingAndPowering();
        if (enemy.Target != null)
        {
            AIManager.Instance.MakeAgentCircleTarget(enemy.Target.transform);
        }
    }

    string IState.GetStateName()
    {
        return "FollowState";
    }
}
