using System.Collections;
using UnityEngine;


class AttackState : IState
{
    private Enemy enemy;

    public void Enter(Enemy enemy)
    {
        this.enemy = enemy;

        if (enemy.Agent.isOnNavMesh && enemy.Agent.enabled) enemy.Stop();
        
        EnemyAttackBehavior();

        EventManager.Instance.onEnemyDecideNextMove += EnemyAttackBehavior;
    }

    public void Exit()
    {
        if (enemy.Agent.isOnNavMesh && enemy.Agent.enabled) enemy.Agent.isStopped = false;
        enemy.Animator.SetFloat("AttackAndPower", 0f);
    }

    public void Update()
    {
        enemy.transform.LookAt(enemy.Target);
    }

    public string GetStateName()
    {
        return "AttackState";
    }

    public void EnemyAttackBehavior()
    {
        if (enemy.isPowerOnCooldown)
        {
            enemy.SetTriggerSingle("TriggerAttack");
        }
        else
        {
            enemy.SetTriggerSingle("TriggerPower");
        }
    }


}
