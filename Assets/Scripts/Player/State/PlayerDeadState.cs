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
        player.isInvincible = true;

        player.audioManager.StopBGM();

        deadPushTime = 0f;
        isPlayWinner = false;

        player.GenerateBlood();

        player.deathCount.Value = player.deathCount.Value + 1;
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
        //player.isInvincible = false;
    }

    public override void FixedUpdate()
    {
        deadPushTime += Time.fixedDeltaTime;

        if (player.deathCount.Value < 5)
        {
            if (deadPushTime > 0.4f)
            {
                Vector2 speedDif = Vector2.zero - controller.rb.velocity;
                float speedDist = speedDif.sqrMagnitude;
                float speedAmount = Mathf.Pow(Mathf.Abs(speedDist) * 24f, 0.9f);
                controller.rb.AddForce(speedAmount * speedDif.normalized);

                //player.audioManager.MuteSfx();
            }

            if (deadPushTime > 1.5f && !isPlayWinner)
            {
                //isPlayWinner = true;
                //player.winnerCanvas.SetActive(true);
                player.winnerCanvas.GetComponentInChildren<WinnerUIAnim>().ChangeWinnerSprite(player.playerIndex);
                //player.endMenu.SetActive(true);
            }
            if(deadPushTime > 3f)
            {
                player.winnerCanvas.GetComponentInChildren<WinnerUIAnim>().ReturnCam();
                player.HidePlayer();
                stateMachine.ChangeState(player.respawnState);
            }
        }
        if (player.deathCount.Value > 4)
        {
            if (deadPushTime > 0.4f)
            {
                Vector2 speedDif = Vector2.zero - controller.rb.velocity;
                float speedDist = speedDif.sqrMagnitude;
                float speedAmount = Mathf.Pow(Mathf.Abs(speedDist) * 24f, 0.9f);
                controller.rb.AddForce(speedAmount * speedDif.normalized);

                //player.audioManager.MuteSfx();
            }

            if (deadPushTime > 1.5f && !isPlayWinner)
            {
                isPlayWinner = true;
                player.winnerCanvas.SetActive(true);
                player.winnerCanvas.GetComponentInChildren<WinnerUIAnim>().ChangeWinnerSprite(player.playerIndex);
                player.endMenu.SetActive(true);
            }
        }
    }

    public override void Update()
    {
    }
}
