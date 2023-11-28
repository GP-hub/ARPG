using UnityEngine;

class IdleState : IState
{
    private Enemy enemy;

    public void Enter(Enemy enemy)
    {
        Debug.Log("Enter: IdleState");
        this.enemy = enemy;
        this.enemy.Target = null;
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
}
