using UnityEngine;

class AttackState : IState
{
    private Enemy enemy;
    private int? forcedAbilityIndex = null;

    public AttackState() { }

    public AttackState(int abilityIndex)
    {
        forcedAbilityIndex = abilityIndex;
    }

    void IState.Enter(Enemy enemy)
    {
        this.enemy = enemy;

        if (forcedAbilityIndex.HasValue)
        {
            enemy.ForceAbility(forcedAbilityIndex.Value); // ← You implement this method
        }
        else
        {
            enemy.DecideNextAbility();
        }

        enemy.SetBoolSingle("TriggerAttack");
        enemy.TargetPosition = enemy.GetInaccurateTarget(enemy.Target.position);

        if (enemy.Agent.isOnNavMesh && enemy.Agent.enabled)
            enemy.Stop();

        enemy.Animator.SetFloat("AttackTree", enemy.GetCurrentAbilityIndex());

        enemy.ResetPerformedBehavior();
    }

    void IState.Exit()
    {
        if (enemy.Agent.isOnNavMesh && enemy.Agent.enabled) enemy.Agent.isStopped = false;

        enemy.ResetSingleBool("TriggerAttack");

        enemy.ResetAttackingAndPowering();
    }
    void IState.Update()
    {
        if (enemy.Target == null)
        {
            enemy.ChangeState(new IdleState());
        }
        if (enemy.isCharging) return;

        Utility.RotateTowardsTarget(enemy.transform, enemy.TargetPosition, enemy.RotationSpeed);
    }

    string IState.GetStateName()
    {
        return "AttackState";
    }

}
