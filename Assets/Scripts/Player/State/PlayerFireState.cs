using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFireState : PlayerState
{
    public PlayerFireState(PlayerStateMachine stateMachine, Player player, string animatorName) : base(stateMachine, player, animatorName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        player.arrow.stateMachine.ChangeState(player.arrow.fireState);
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public override void Update()
    {
        base.Update();
        stateMachine.ChangeState(player.idleState);
    }
}
