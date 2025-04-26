
class AttackState : IState
{
    private Enemy enemy;

    void IState.Enter(Enemy enemy)
    {
        this.enemy = enemy;

        if (enemy.Agent.isOnNavMesh && enemy.Agent.enabled) enemy.Stop();

        enemy.SetBoolSingle("TriggerAttack");
        enemy.DecideNextAbility();
        // Only work if enemy Attack state as the AttackTree parameter and a blend tree for attack State animation
        enemy.Animator.SetFloat("AttackTree", enemy.BlendTreeThreshold());

    }

    void IState.Exit()
    {
        if (enemy.Agent.isOnNavMesh && enemy.Agent.enabled) enemy.Agent.isStopped = false;

        //enemy.Animator.SetFloat("AttackAndPower", 0f);

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
        //if (!enemy.OffCooldownAbilities.Contains(enemy.CurrentAbility))
        //{
        //    enemy.ChangeState(new IdleState());
        //    return;
        //}

        //enemy.transform.LookAt(enemy.Target);
        Utility.RotateTowardsTarget(enemy.transform, enemy.Target, enemy.RotationSpeed);
    }

    string IState.GetStateName()
    {
        return "AttackState";
    }

}
