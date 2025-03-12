
public class ArrowState
{
    public ArrowStateMachine stateMachine;
    public Arrow arrow;
    public string animatorName;

    public ArrowState(ArrowStateMachine stateMachine, Arrow arrow, string animatorName)
    {
        this.stateMachine = stateMachine;
        this.arrow = arrow;
        this.animatorName = animatorName;
    }

    public virtual void Enter()
    {
        //player.animator.SetBool(animatorName, true);
    }

    public virtual void Update()
    {
    }

    public virtual void FixedUpdate()
    {
    }

    public virtual void Exit()
    {
        //player.animator.SetBool(animatorName, false);
    }
}
