using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveState : PlayerState
{
    public PlayerMoveState(PlayerStateMachine stateMachine, Player player, string animatorName) : base(stateMachine, player, animatorName)
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
        controller.PlayerMove();
    }

    public override void Update()
    {
        base.Update();
        if (!controller.isMoving())
            stateMachine.ChangeState(player.idleState);
        if (controller.isDashing)
            stateMachine.ChangeState(player.dashInState);
    }
}
