
using UnityEngine;

public class ArrowStopState : ArrowState
{
    public ArrowStopState(ArrowStateMachine stateMachine, Arrow arrow, string animatorName) : base(stateMachine, arrow, animatorName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        arrow.InitArrow();
        arrow.col.isTrigger = true;
        arrow.rb.simulated = true;
    }

    public override void Exit()
    {
        base.Exit();
        arrow.trailRenderer.enabled = false;
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        Vector2 speedDif = Vector2.zero - arrow.rb.velocity;
        float speedDist = speedDif.sqrMagnitude;
        float speedAmount = Mathf.Pow(Mathf.Abs(speedDist) * 5f, 0.8f);
        arrow.rb.AddForce(speedAmount * speedDif.normalized);
    }

    public override void Update()
    {
        base.Update();
        if (arrow.getArrowPlayer && arrow.getArrowPlayer.arrowCount < 2)
        {
            arrow.getArrowPlayer.arrowCount++;
            arrow.audioManager.PlaySfx(arrow.audioManager.pickup);
            arrow.transform.parent = arrow.getArrowPlayer.transform;
            arrow.player = arrow.getArrowPlayer;
            stateMachine.ChangeState(arrow.aimState);
        }
    }
}
