using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class PlayerRespawnState : PlayerState
{
    private float selectTimer = 0.2f;
    private TMP_Text countdownText;
    private float countdown = 3f;
    private bool isCountdownStarted = false;

    public PlayerRespawnState(PlayerStateMachine stateMachine, Player player, string animatorName) : base(stateMachine, player, animatorName)
    {
    }
    public override void Enter()
    {
        
        Debug.Log("enterRespawnState");
        base.Enter();
        player.isChoosing = true;

        countdownText = GameObject.Find("CountDownText" + player.playerIndex).GetComponent<TMP_Text>();
        countdownText.gameObject.SetActive(false);
        isCountdownStarted = false;
    }

    public override void Exit()
    {
        base.Exit();
    }
    public override void Update()
    {
                Debug.Log(isCountdownStarted);
        base.Update();
        selectTimer -= Time.deltaTime;

        if (!isCountdownStarted && selectTimer < 0)
        {
            player.controller.PlayerRespawn();
            selectTimer = 0.2f;
            player.controller.PlayerSubmit();
            if (!player.isChoosing) // 说明已提交成功
            {
                isCountdownStarted = true;
                player.StartCoroutine(ShowRespawnCountdownAndSpawn());
            }
        }
    }
    private IEnumerator ShowRespawnCountdownAndSpawn()
    {
        countdownText.gameObject.SetActive(true);

        float timeLeft = countdown;
        while (timeLeft > 0)
        {
            countdownText.text = Mathf.CeilToInt(timeLeft).ToString();
            yield return new WaitForSeconds(1f);
            timeLeft -= 1f;
        }

        countdownText.gameObject.SetActive(false);

        Vector3Int selectedCell = player.controller.GetSelectedCell();
        Vector3 respawnWorldPos = player.controller.GetTilemap().GetCellCenterWorld(selectedCell);
        player.controller.SpawnPlayer(player.transform.position);
        isCountdownStarted = false;
    }
}
