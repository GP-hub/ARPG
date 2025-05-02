
class PowerState : IState
{
    private Enemy enemy;

    void IState.Enter(Enemy enemy)
    {
        this.enemy = enemy;

        enemy.StartCoroutine(enemy.DelayedPowerEnter());
    }

    void IState.Exit()
    {
        if (enemy.Agent.isOnNavMesh && enemy.Agent.enabled) enemy.Agent.isStopped = false;

        enemy.ResetSingleBool("TriggerPower");

        enemy.ResetAttackingAndPowering();
    }
    void IState.Update()
    {
        if (enemy.IsAttacking || enemy.IsPowering) return;

        Utility.RotateTowardsTarget(enemy.transform, enemy.Target, enemy.RotationSpeed);
    }

    string IState.GetStateName()
    {
        return "PowerState";
    }

}
