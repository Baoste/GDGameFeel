using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ArrowBrokenState : ArrowState
{
    private float destroyTime = 0f;
    private PlayerController controller;
    public ArrowBrokenState(ArrowStateMachine stateMachine, Arrow arrow, string animatorName) : base(stateMachine, arrow, animatorName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        controller = arrow.player.controller;
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
        ArrowRotate();
        destroyTime += Time.deltaTime;
        if (destroyTime >= 2f)
        {
            destroyTime = 0f;
            arrow.waveGenerator.transform.position = arrow.transform.position;
            arrow.waveGenerator.CallShockWave();
            arrow.arrowGenerator.DestroyArrow(arrow.gameObject, arrow.transform.position);
        }
    }

    private void ArrowRotate()
    {
        if (controller.aimVec.magnitude > 0.5f)
        {
            arrow.aimDirection = controller.aimVec.normalized;
            Vector3 rotatedPos = new Vector3(
                arrow.lenToPlayer * arrow.aimDirection.x,
                arrow.lenToPlayer * arrow.aimDirection.y,
                0
            );
            arrow.transform.localPosition = rotatedPos + Vector3.up * Random.Range(-.1f, .1f);
            float angle = Mathf.Atan2(arrow.aimDirection.y, arrow.aimDirection.x) * Mathf.Rad2Deg;
            arrow.transform.localRotation = Quaternion.Euler(0, 0, angle);
        }
    }
}
