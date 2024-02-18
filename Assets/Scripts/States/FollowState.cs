using UnityEngine;

class FollowState : IState
{
    private Enemy enemy;

    public void Enter(Enemy enemy)
    {
        this.enemy = enemy;
        enemy.SetTriggerSingle("TriggerWalk");
        enemy.ResetAttackingAndPowering();
        //enemy.Agent.ResetPath();
    }

    public void Exit()
    {
        enemy.ResetTriggerSingle("TriggerWalk");
    }

    public void Update()
    {
        if (enemy.Target != null)
        {
            AIManager.Instance.MakeAgentCircleTarget(enemy.Target.transform);
        }
    }

    public string GetStateName()
    {
        return "FollowState";
    }
}
