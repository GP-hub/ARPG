using UnityEngine;

public class MoveToState : IState
{
    private Enemy enemy;
    private Vector3 destination;

    public MoveToState(Vector3 destination)
    {
        this.destination = destination;
    }

    void IState.Enter(Enemy enemy)
    {
        this.enemy = enemy;
        enemy.SetBoolSingle("TriggerWalk");
        enemy.ResetAttackingAndPowering();
        enemy.isMoving = true;
    }

    void IState.Exit()
    {
        enemy.ResetSingleBool("TriggerWalk");
        
    }

    void IState.Update()
    {
        AIManager.Instance.MoveUnitsToPosition(destination);

        bool allArrived = AIManager.Instance.MoveUnitsToPosition(destination);

        if (allArrived)
        {
            enemy.isMoving = false;
            enemy.TryUseFollowUpAbility();            
        }
    }

    string IState.GetStateName()
    {
        return "MoveToState";
    }
}
