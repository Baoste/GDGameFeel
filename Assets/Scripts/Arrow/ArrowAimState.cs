
using UnityEngine;

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

        // init rotate
        Vector3 rotatedPos = Vector3.up * arrow.lenToPlayer;
        arrow.transform.localPosition = rotatedPos;
        float angle = Mathf.Atan2(arrow.aimDirection.y, arrow.aimDirection.x) * Mathf.Rad2Deg;
        arrow.transform.localRotation = Quaternion.Euler(0, 0, angle);
    }

    public override void Exit()
    {
        base.Exit();
        arrow.GetComponent<SpriteRenderer>().sortingOrder = 1;
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
            arrow.aimDirection = controller.aimVec.normalized;
            Vector3 rotatedPos = new Vector3(
                arrow.lenToPlayer * arrow.aimDirection.x,
                arrow.lenToPlayer * arrow.aimDirection.y,
                0
            );
            arrow.transform.localPosition = rotatedPos;
            float angle = Mathf.Atan2(arrow.aimDirection.y, arrow.aimDirection.x) * Mathf.Rad2Deg;
            arrow.transform.localRotation = Quaternion.Euler(0, 0, angle);
        }
    }
}
