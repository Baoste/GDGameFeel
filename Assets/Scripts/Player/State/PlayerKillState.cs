using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerKillState : PlayerDeadState
{

    public PlayerKillState(PlayerStateMachine stateMachine, Player player, string animatorName) : base(stateMachine, player, animatorName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        generateScoreParticleTime = 3f;

        player.GenerateBlood();
    }

    public override void Exit()
    {
        base.Exit();
        //player.isInvincible = false;
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public override void Update()
    {
        base.Update();
    }
}
