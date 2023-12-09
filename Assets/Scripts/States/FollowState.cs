using UnityEngine;

class FollowState : IState
{
    private Enemy enemy;

    public void Enter(Enemy enemy)
    {
        this.enemy = enemy;
        enemy.SetTriggerSingle("TriggerWalk");
    }

    public void Exit()
    {

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
