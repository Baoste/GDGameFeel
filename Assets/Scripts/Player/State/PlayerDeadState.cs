using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeadState : PlayerState
{
    public PlayerDeadState(PlayerStateMachine stateMachine, Player player, string animatorName) : base(stateMachine, player, animatorName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        player.stainGenerator.Generate(Color.red, player.transform.position, Vector3.up, 1, 10f);
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
    }
}
