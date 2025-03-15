
public class PlayerState
{
    public PlayerStateMachine stateMachine;
    public Player player;
    public string animatorName;

    public PlayerController controller;

    public PlayerState(PlayerStateMachine stateMachine, Player player, string animatorName)
    {
        this.stateMachine = stateMachine;
        this.player = player;
        this.animatorName = animatorName;
        this.controller = player.controller;
    }

    public virtual void Enter()
    {
        player.animator.SetBool(animatorName, true);
    }


    public virtual void Exit()
    {
        player.animator.SetBool(animatorName, false);
    }

    public virtual void FixedUpdate()
    {
    }
    public virtual void Update()
    {
        if (controller.isFiring && player.arrow)
            stateMachine.ChangeState(player.aimState);
    }
}
