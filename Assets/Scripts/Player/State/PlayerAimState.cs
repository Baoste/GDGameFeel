using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAimState : PlayerState
{
    private float aimTime;
    public PlayerAimState(PlayerStateMachine stateMachine, Player player, string animatorName) : base(stateMachine, player, animatorName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        aimTime = 0;
        player.audioManager.PlayLoopSfx(player.audioManager.lightningReady);
    }

    public override void Exit()
    {
        base.Exit();
        player.audioManager.StopLoopSfx();
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        controller.PlayerAim();
    }

    public override void Update()
    {
        // base.Update();
        if (player.arrow)
        {
            aimTime += Time.deltaTime;
            player.arrow.ChargeUp(aimTime);
            if (aimTime > 4f)
            {
                player.arrow.stateMachine.ChangeState(player.arrow.brokenState);
            }
        }
        else
        {
            stateMachine.ChangeState(player.idleState);
        }

        if (controller.isFire)
            stateMachine.ChangeState(player.fireState);
        // dash stop aiming
        if (controller.isDashing && player.dashCoolTime <= 0f)
        {
            player.arrow.InitArrow();
            player.canAim = false;
            stateMachine.ChangeState(player.dashInState);
        }
    }
}
