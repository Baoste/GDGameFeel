using DG.Tweening;
using System.Collections;
using UnityEngine;

public class PlayerFallState : PlayerState
{
    private float deadPushTime;
    private bool isPlayWinner;
    public PlayerFallState(PlayerStateMachine stateMachine, Player player, string animatorName) : base(stateMachine, player, animatorName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        player.audioManager.StopBGM();

        deadPushTime = 0f;
        isPlayWinner = false;

        player.audioManager.PlaySfx(player.audioManager.playerFall);
        if (player.arrow)
        {
            player.arrow.transform.parent = player.transform.parent;
            player.arrow.stateMachine.ChangeState(player.arrow.stopState);
        }
        player.transform.DOScale(Vector3.zero, 1f);
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
        deadPushTime += Time.deltaTime;
        if (deadPushTime > 1.5f && !isPlayWinner)
        {
            isPlayWinner = true;
            player.winnerCanvas.SetActive(true);
            player.winnerCanvas.GetComponentInChildren<WinnerUIAnim>().ChangeWinnerSprite(player.playerIndex);
            player.endMenu.SetActive(true);
        }
    }
}
