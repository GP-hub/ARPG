using UnityEngine;


class AttackState : IState
{
    private Enemy enemy;

    public void Enter(Enemy enemy)
    {
        this.enemy = enemy;

        if (enemy.Agent.isOnNavMesh && enemy.Agent.enabled) enemy.Stop();
    }

    public void Exit()
    {
        if (enemy.Agent.isOnNavMesh && enemy.Agent.enabled) enemy.Agent.isStopped = false;
    }

    public void Update()
    {
        enemy.transform.LookAt(enemy.Target);
    }
}
