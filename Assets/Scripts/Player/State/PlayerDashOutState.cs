using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDashOutState : PlayerDashState
{
    public PlayerDashOutState(PlayerStateMachine stateMachine, Player player, string animatorName) : base(stateMachine, player, animatorName)
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
        dashDuring += Time.fixedDeltaTime;
        if (dashDuring > controller.dashDecTime)
            stateMachine.ChangeState(player.idleState);
        else
            controller.PlayerDashOut();
    }

    public override void Update()
    {
        base.Update();
    }
}
