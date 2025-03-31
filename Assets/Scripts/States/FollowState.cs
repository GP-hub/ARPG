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
        enemy.ResetAttackingAndPowering();
        if (enemy.Target != null)
        {
            AIManager.Instance.MakeAgentCircleTarget(enemy.Target.transform);
        }
        //Debug.Log("target: " + enemy.Target);
    }

    string IState.GetStateName()
    {
        return "FollowState";
    }
}
