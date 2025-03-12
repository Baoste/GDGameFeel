using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDashInState : PlayerDashState
{
    public PlayerDashInState(PlayerStateMachine stateMachine, Player player, string animatorName) : base(stateMachine, player, animatorName)
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
        dashDuring += Time.deltaTime;
        if (dashDuring > controller.dashAccTime)
            stateMachine.ChangeState(player.dashingState);
        else
            controller.PlayerDashIn();
    }

    public override void Update()
    {
        base.Update();
    }
}
