
using UnityEngine;

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
        FlipSprite();
        if (player.canAim && controller.isFiring && player.arrow)
        {
            stateMachine.ChangeState(player.aimState);
        }
    }

    private void FlipSprite()
    {
        if (controller.aimVec.x > 0)
        {
            player.spriteTrans.localScale = new Vector3(1, 1, 1);
        }
        else if (controller.aimVec.x < 0)
        {
            player.spriteTrans.localScale = new Vector3(-1, 1, 1);
        }
    }
}
