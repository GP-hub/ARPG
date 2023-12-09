using UnityEngine;

class IdleState : IState
{
    private Enemy enemy;

    public void Enter(Enemy enemy)
    {
        this.enemy = enemy;
        this.enemy.Target = null;
        enemy.SetTriggerSingle("TriggerIdle");
    }

    public void Exit()
    {
        
    }

    public void Update()
    {
        if (enemy.Target != null)
        {
            enemy.ChangeState(new FollowState());
        }
    }

    public string GetStateName()
    {
        return "IdleState";
    }

}
