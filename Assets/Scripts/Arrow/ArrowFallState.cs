using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowFallState : ArrowState
{
    public ArrowFallState(ArrowStateMachine stateMachine, Arrow arrow, string animatorName) : base(stateMachine, arrow, animatorName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        arrow.trailRenderer.enabled = true;
        arrow.arrowLight.intensity = 0.5f;
        
        float axisX = arrow.transform.position.x;
        Vector3 targetPos = arrow.generatePos;

        arrow.GenerateShadow(targetPos + Vector3.down * 0.8f);
        arrow.transform.DOMove(targetPos, 1f).SetEase(Ease.InQuad).OnComplete(() =>
        {
            arrow.col.isTrigger = true;
            arrow.rb.simulated = true;
            arrow.GetComponent<SpriteRenderer>().sprite = arrow.onfloor;
            arrow.arrowLight.lightCookieSprite = arrow.onfloor;

            // effect
            arrow.lightParticle.Play();
            arrow.impulseSource.GenerateImpulse();
            arrow.GenerateLightning();
        });
    }

    public override void Exit()
    {
        base.Exit();
        arrow.trailRenderer.enabled = false;
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public override void Update()
    {
        base.Update();

        if (arrow.getArrowPlayer && arrow.getArrowPlayer.arrowCount < 2)
        {
            arrow.GetComponent<SpriteRenderer>().sprite = arrow.normal;
            arrow.arrowLight.lightCookieSprite = arrow.normal;
            arrow.arrowLight.intensity = 0f;

            arrow.audioManager.PlaySfx(arrow.audioManager.pickup);
            arrow.getArrowPlayer.arrowCount++;
            arrow.transform.parent = arrow.getArrowPlayer.transform;
            arrow.player = arrow.getArrowPlayer;
            stateMachine.ChangeState(arrow.aimState);
        }
    }

}
