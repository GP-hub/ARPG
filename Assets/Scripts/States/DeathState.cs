
public class DeathState : IState
{
    private Enemy enemy;

    void IState.Enter(Enemy enemy)
    {
        this.enemy = enemy;
        enemy.gameObject.tag = "Dead";
    }

    void IState.Exit()
    {

    }
    void IState.Update()
    {

    }

    string IState.GetStateName()
    {
        return "DeathState";
    }

}
