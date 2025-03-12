using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowStopState : ArrowState
{
    public ArrowStopState(ArrowStateMachine stateMachine, Arrow arrow, string animatorName) : base(stateMachine, arrow, animatorName)
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
        Vector2 targetSpeed = Vector2.zero;
        Vector2 dashDif = targetSpeed - arrow.rb.velocity;
        float dashDist = dashDif.sqrMagnitude;
        float dashAmount = Mathf.Pow(Mathf.Abs(dashDist) * 10f, 0.8f);
        arrow.rb.AddForce(dashAmount * dashDif.normalized);
        if (arrow.rb.velocity.sqrMagnitude < 0.2f)
            arrow.col.isTrigger = true;
    }

    public override void Update()
    {
        base.Update();
        if (arrow.getArrowPlayer)
        {
            arrow.transform.parent = arrow.getArrowPlayer.transform;
            arrow.player = arrow.getArrowPlayer;
            stateMachine.ChangeState(arrow.aimState);
        }
    }
}
