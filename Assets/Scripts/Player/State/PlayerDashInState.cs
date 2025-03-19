
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

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
        player.DashFreeze();
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        if (dashDuring > controller.dashAccTime)
            stateMachine.ChangeState(player.dashingState);
        else
            controller.PlayerDashIn();
    }

    public override void Update()
    {
        base.Update();
        dashDuring += Time.deltaTime;
    }
}
