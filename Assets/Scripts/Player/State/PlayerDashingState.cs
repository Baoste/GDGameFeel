
using UnityEngine;


public class PlayerDashingState : PlayerDashState
{
    private float generateDeltime;
    public PlayerDashingState(PlayerStateMachine stateMachine, Player player, string animatorName) : base(stateMachine, player, animatorName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        player.forceField.enabled = true;

        Vector3 impulseDir = controller.rb.velocity.normalized;
        player.impulseSource.m_DefaultVelocity = impulseDir * 0.15f;
        player.impulseSource.GenerateImpulse();

        player.audioManager.PlaySfx(player.audioManager.dash);
        
        //player.GenerateShadow();
        generateDeltime = controller.dashTime / 4;

    }

    public override void Exit()
    {
        base.Exit();
        player.forceField.enabled = false;
        player.GenerateShadow();
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        dashDuring += Time.fixedDeltaTime;
        if (dashDuring > controller.dashTime)
            stateMachine.ChangeState(player.dashOutState);
        else
            controller.PlayerDashTime();
    }

    public override void Update()
    {
        base.Update();
        if (dashDuring > generateDeltime)
        {
            player.GenerateShadow();
            generateDeltime += controller.dashTime / 4;
        }
    }

    

}
