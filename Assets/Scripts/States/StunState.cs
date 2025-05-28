using UnityEngine;

public class StunState : IState
{
    private Enemy enemy;
    private GameObject stunEffect;

    void IState.Enter(Enemy enemy)
    {
        this.enemy = enemy;
        enemy.IsCC = true;
        if (enemy.Agent.isOnNavMesh && enemy.Agent.enabled) enemy.Stop();
        enemy.SetBoolSingle("TriggerStun");
        enemy.ResetAttackingAndPowering();

        Vector3 enemyTopPosition = enemy.transform.position + Vector3.up * (enemy.transform.localScale.y + 0.85f);
        stunEffect = PoolingManagerSingleton.Instance.GetObjectFromPool("Stun_loop", enemyTopPosition);
    }

    void IState.Exit()
    {
        enemy.IsCC = false;
        if (enemy.Agent.isOnNavMesh && enemy.Agent.enabled) enemy.Agent.isStopped = false;
        enemy.ResetSingleBool("TriggerStun");
        enemy.StartCastCooldown();
    }
    void IState.Update()
    {
        UpdateCountdown();
    }

    private void UpdateCountdown()
    {
        enemy.CCDuration -= Time.deltaTime;

        // If CCduration reaches zero or below, exit the state
        if (enemy.CCDuration <= 0)
        {
            stunEffect.SetActive(false);
            enemy.ChangeState(new IdleState());
        }
    }

    string IState.GetStateName()
    {
        return "StunState";
    }
}
