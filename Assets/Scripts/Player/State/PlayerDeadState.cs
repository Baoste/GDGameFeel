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
        player.GenerateBlood();
        if (player.arrow)
        {
            player.arrow.transform.parent = player.transform.parent;
            player.arrow.stateMachine.ChangeState(player.arrow.stopState);
        }
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void FixedUpdate()
    {
        Vector2 targetSpeed = Vector2.zero;
        Vector2 dashDif = targetSpeed - controller.rb.velocity;
        float dashDist = dashDif.sqrMagnitude;
        float dashAmount = Mathf.Pow(Mathf.Abs(dashDist) * 24f, 0.9f);
        controller.rb.AddForce(dashAmount * dashDif.normalized);
    }

    public override void Update()
    {
    }
}
