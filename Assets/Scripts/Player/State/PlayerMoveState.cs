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
        player.forceField.enabled = true;
        player.dustEffect.Play();
    }

    public override void Exit()
    {
        base.Exit();
        player.forceField.enabled = false;
        player.dustEffect.Stop();
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
        if (controller.isDashing && player.dashCoolTime <= 0f)
            stateMachine.ChangeState(player.dashInState);
    }
}
