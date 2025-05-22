public interface IState
{
    void Enter(Enemy parent);
    void Update();
    void Exit();
    string GetStateName();
}