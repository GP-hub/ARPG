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
        EventManager.Instance.onEnemyDecideNextMove -= EnemyAttackBehavior;
        if (enemy.Agent.isOnNavMesh && enemy.Agent.enabled) enemy.Agent.isStopped = false;
        enemy.Animator.SetFloat("AttackAndPower", 0f);
        enemy.ResetTriggerSingle("TriggerAttack");
        enemy.ResetTriggerSingle("TriggerPower");
        enemy.ResetAttackingAndPowering();
    }

    public void Update()
    {
        if (enemy.isCharging) return;

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
