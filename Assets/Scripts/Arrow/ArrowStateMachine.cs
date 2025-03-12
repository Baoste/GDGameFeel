
public class ArrowStateMachine
{
    public ArrowState currentState;
    public void Initialize(ArrowState state)
    {
        currentState = state;
        currentState.Enter();
    }

    public void ChangeState(ArrowState state)
    {
        currentState.Exit();
        currentState = state;
        currentState.Enter();
    }
}
