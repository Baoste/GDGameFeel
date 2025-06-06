
using DG.Tweening;
using UnityEngine;
using UnityEngine.Tilemaps;

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
            arrow.audioManager.PlaySfx(arrow.audioManager.pickup);
            arrow.transform.parent = arrow.getArrowPlayer.transform;
            arrow.player = arrow.getArrowPlayer;
            stateMachine.ChangeState(arrow.aimState);
        }
        if (!arrow.isOutFloor && !arrow.ExamFloor())
        {
            arrow.isOutFloor = true;
            arrow.GetComponent<SpriteRenderer>().DOFade(0f, 0.5f);
            arrow.arrowGenerator.DestroyArrow(arrow.gameObject, arrow.transform.position);
        }
    }

    
}
