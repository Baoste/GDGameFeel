using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDashingState : PlayerDashState
{
    public PlayerDashingState(PlayerStateMachine stateMachine, Player player, string animatorName) : base(stateMachine, player, animatorName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        Vector3 impulseDir = controller.rb.velocity.normalized;
        player.impulseSource.m_DefaultVelocity = impulseDir * 0.1f;
        player.impulseSource.GenerateImpulse();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        dashDuring += Time.deltaTime;
        if (dashDuring > controller.dashTime)
            stateMachine.ChangeState(player.dashOutState);
        else
            controller.PlayerDashTime();
    }

    public override void Update()
    {
        base.Update();
    }
}
