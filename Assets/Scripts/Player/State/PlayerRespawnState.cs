using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Tilemaps;


public class PlayerRespawnState : PlayerState
{
    private float selectTimer = 0.2f;
    //private TMP_Text countdownText;
    private float countdown = 5f;
    private bool isCountdownStarted = false;

    public PlayerRespawnState(PlayerStateMachine stateMachine, Player player, string animatorName) : base(stateMachine, player, animatorName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        player.isInvincible = true;
        player.isChoosing = true;

        //selectTimer = 0.2f;
        countdown = 5f;
        isCountdownStarted = false;

        var cameraData = Camera.main.GetUniversalAdditionalCameraData();
        cameraData.SetRenderer(0);

        // 清除空格状态
        bool tmp = player.controller.isSubmit;
    }

    public override void Exit()
    {
        base.Exit();
        //player.isInvincible = false;
        player.audioManager.PlayBGM();
        player.InvincibilityRoutine(player.invincibleDuration);

        player.transform.localScale = Vector3.one;
        player.playerFoot.isFall = false;
        player.dashCoolTime = 0.5f;
    }

    public override void Update()
    {
        base.Update();

        selectTimer -= Time.deltaTime;
        countdown -= Time.deltaTime;
        
        // 冷却结束，自动复活 
        if (countdown <= 0)
        {
            Vector3Int selectedCell = player.controller.selectedCell;
            Vector3 respawnWorldPos = player.controller.tilemap.GetCellCenterWorld(selectedCell);
            player.controller.SpawnPlayer(respawnWorldPos);
            isCountdownStarted = false;
            player.controller.InitCell();
        }

        if (!isCountdownStarted && selectTimer < 0)
        {
            selectTimer = 0.1f;
            player.controller.PlayerRespawn();
            player.controller.PlayerSubmit();
            if (!player.isChoosing)
            {
                isCountdownStarted = true;
                //player.StartCoroutine(ShowRespawnCountdownAndSpawn());
            }
        }
    }

    //private IEnumerator ShowRespawnCountdownAndSpawn()
    //{

    //    float timeLeft = countdown;
    //    while (timeLeft > 0)
    //    {
    //        //countdownText.text = Mathf.CeilToInt(timeLeft).ToString();
    //        yield return new WaitForSeconds(1f);
    //        timeLeft -= 1f;
    //    }

    //    //countdownText.text = "";

    //    Vector3Int selectedCell = player.controller.GetSelectedCell();
    //    Vector3 respawnWorldPos = player.controller.GetTilemap().GetCellCenterWorld(selectedCell);
    //    player.controller.SpawnPlayer(player.transform.position);
    //    isCountdownStarted = false;
    //}

    //private IEnumerator RespawnCountDown()
    //{
    //    yield return new WaitForSeconds(3f);

    //    Vector3Int selectedCell = player.controller.GetSelectedCell();
    //    Vector3 respawnWorldPos = player.controller.GetTilemap().GetCellCenterWorld(selectedCell);
    //    player.controller.SpawnPlayer(respawnWorldPos);
    //    isCountdownStarted = false;
    //    player.controller.InitCell();
    //}
}
