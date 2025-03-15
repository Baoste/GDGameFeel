using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.U2D;

public class PlayerDashingState : PlayerDashState
{
    private float generateDeltime;
    public PlayerDashingState(PlayerStateMachine stateMachine, Player player, string animatorName) : base(stateMachine, player, animatorName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        player.GenerateShadow();
        generateDeltime = controller.dashTime / 3;

        Vector3 impulseDir = controller.rb.velocity.normalized;
        player.impulseSource.m_DefaultVelocity = impulseDir * 0.1f;
        player.impulseSource.GenerateImpulse();
    }

    public override void Exit()
    {
        base.Exit();
        player.GenerateShadow();
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
        if (dashDuring > generateDeltime)
        {
            player.GenerateShadow();
            generateDeltime += controller.dashTime / 3;
        }
    }

    

}
