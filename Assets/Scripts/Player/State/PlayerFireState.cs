using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFireState : PlayerState
{
    private float fireTime;
    public PlayerFireState(PlayerStateMachine stateMachine, Player player, string animatorName) : base(stateMachine, player, animatorName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        player.arrowCount--;
        fireTime = 0;
        if (player.arrow.isFast)
            player.audioManager.PlaySfx(player.audioManager.fireFast);
        else
            player.audioManager.PlaySfx(player.audioManager.fire);
        player.arrow.stateMachine.ChangeState(player.arrow.fireState);
    }

    public override void Exit()
    {
        base.Exit();
        controller.SetGamepadMotor(0);
        controller.recoil = 3f;
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        controller.PlayerFire();
    }

    public override void Update()
    {
        base.Update();
        fireTime += Time.deltaTime;
        if (fireTime > 0.2f)
            stateMachine.ChangeState(player.idleState);
    }
}
