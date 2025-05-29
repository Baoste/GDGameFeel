using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleState : PlayerState
{
    public PlayerIdleState(PlayerStateMachine stateMachine, Player player, string animatorName) : base(stateMachine, player, animatorName)
    {
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        controller.PlayerIdle();
    }

    public override void Update()
    {
        base.Update();
        if (controller.isMoving())
            stateMachine.ChangeState(player.moveState);
        if (controller.isDashing && player.dashCoolTime <= 0f)
            stateMachine.ChangeState(player.dashInState);
    }
}
