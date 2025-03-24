using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeadState : PlayerState
{
    private float deadPushTime;

    private bool isPlayWinner;
    public PlayerDeadState(PlayerStateMachine stateMachine, Player player, string animatorName) : base(stateMachine, player, animatorName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        player.audioManager.StopBGM();

        deadPushTime = 0f;
        isPlayWinner = false;

        player.GenerateBlood();
        // drop arrow
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
        deadPushTime += Time.fixedDeltaTime;
        if (deadPushTime > 0.4f)
        {
            Vector2 speedDif = Vector2.zero - controller.rb.velocity;
            float speedDist = speedDif.sqrMagnitude;
            float speedAmount = Mathf.Pow(Mathf.Abs(speedDist) * 24f, 0.9f);
            controller.rb.AddForce(speedAmount * speedDif.normalized);
            
            player.audioManager.MuteSfx();
        }
        if (deadPushTime > 1.5f && !isPlayWinner)
        {
            isPlayWinner = true;
            player.winnerCanvas.SetActive(true);
            player.winnerCanvas.GetComponentInChildren<WinnerUIAnim>().ChangeWinnerSprite(player.playerIndex);
            player.endMenu.SetActive(true);
        }

    }

    public override void Update()
    {
    }
}
