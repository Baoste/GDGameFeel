using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowFireState : ArrowState
{
    private float flyTime;
    public ArrowFireState(ArrowStateMachine stateMachine, Arrow arrow, string animatorName) : base(stateMachine, arrow, animatorName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        arrow.transform.parent = arrow.player.transform.parent;
        arrow.player = null;
        flyTime = 0;
        arrow.rb.simulated = true;
        arrow.col.isTrigger = false;
        float amount = 40f;
        arrow.rb.AddForce(amount * arrow.aimDirection, ForceMode2D.Impulse);
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
        flyTime += Time.deltaTime;
        if (flyTime > .8f)
            stateMachine.ChangeState(arrow.stopState);
    }
}
