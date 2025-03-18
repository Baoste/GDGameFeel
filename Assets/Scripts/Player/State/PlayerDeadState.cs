using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeadState : PlayerState
{
    public PlayerDeadState(PlayerStateMachine stateMachine, Player player, string animatorName) : base(stateMachine, player, animatorName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        player.audioManager.PlaySfx(player.audioManager.hitPlayer);
        player.endMenu.SetActive(true);
        player.GenerateBlood();
        if (player.arrow)
        {
            player.arrow.transform.parent = player.transform.parent;
            player.arrow.stateMachine.ChangeState(player.arrow.stopState);
        }
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void FixedUpdate()
    {
        Vector2 speedDif = Vector2.zero - controller.rb.velocity;
        float speedDist = speedDif.sqrMagnitude;
        float speedAmount = Mathf.Pow(Mathf.Abs(speedDist) * 24f, 0.9f);
        controller.rb.AddForce(speedAmount * speedDif.normalized);
    }

    public override void Update()
    {
    }
}
