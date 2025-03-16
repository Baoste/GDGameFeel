using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;
using UnityEngine.InputSystem.XR;
using UnityEngine.UIElements;

public class ArrowAimState : ArrowState
{
    
    private PlayerController controller;

    public ArrowAimState(ArrowStateMachine stateMachine, Arrow arrow, string animatorName) : base(stateMachine, arrow, animatorName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        controller = arrow.player.controller;
        arrow.aimDirection = Vector2.up;
        arrow.rb.simulated = false;
        arrow.col.isTrigger = true;
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
        if (arrow.player != null)
        {
            ArrowRotate();
        }
    }

    private void ArrowRotate()
    {
        if (controller.aimVec.magnitude > 0.5f)
        {
            float len = 1.1f;
            arrow.aimDirection = controller.aimVec.normalized;
            Vector3 rotatedPos = new Vector3(
                len * arrow.aimDirection.x,
                len * arrow.aimDirection.y,
                0
            );
            arrow.transform.localPosition = rotatedPos;
            float angle = Mathf.Atan2(arrow.aimDirection.y, arrow.aimDirection.x) * Mathf.Rad2Deg;
            arrow.transform.localRotation = Quaternion.Euler(0, 0, angle - 45);
        }
    }
}
