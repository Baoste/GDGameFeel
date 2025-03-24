
using UnityEngine;


public class ArrowFireState : ArrowState
{
    private float flyTime;
    private Vector3 firePos;
    public ArrowFireState(ArrowStateMachine stateMachine, Arrow arrow, string animatorName) : base(stateMachine, arrow, animatorName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        arrow.fireParticle.Play();
        arrow.trailRenderer.enabled = true;

        firePos = arrow.transform.position;
        arrow.transform.parent = arrow.player.transform.parent;
        arrow.player = null;
        flyTime = 0;

        arrow.rb.simulated = true;
        arrow.col.isTrigger = false;
        arrow.rb.AddForce(arrow.fireForce * arrow.aimDirection, ForceMode2D.Impulse);
    }

    public override void Exit()
    {
        base.Exit();
        if (arrow.isFast)
        {
            arrow.arrowGenerator.DestroyArrow(arrow.gameObject, firePos);
            arrow.InitArrow();
        }
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public override void Update()
    {
        base.Update();
        ArrowRotate();
        flyTime += Time.deltaTime;
        if (flyTime > arrow.flyTime)
            stateMachine.ChangeState(arrow.stopState);
    }

    private void ArrowRotate()
    {
        Vector2 dirction = arrow.rb.velocity;
        float angle = Mathf.Atan2(dirction.y, dirction.x) * Mathf.Rad2Deg;
        arrow.transform.localRotation = Quaternion.Euler(0, 0, angle);
    }
}
