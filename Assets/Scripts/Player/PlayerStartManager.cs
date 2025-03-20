using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerStartManager : MonoBehaviour
{
    public SpriteRenderer leftSp;
    public SpriteRenderer rightSp;
    public Canvas videoCanvas;

    private int readyCount = 0;


    private void OnPlayerJoined(PlayerInput newPlayer)
    {
        readyCount += 1;
        leftSp.color = Color.green;
        if (readyCount == 2)
        {
            rightSp.color = Color.green;
            GameStart();
        }
    }

    private void GameStart()
    {
        videoCanvas.gameObject.SetActive(false);
        leftSp.DOFade(0, 1f);
        rightSp.DOFade(0, 1f);
        FindObjectOfType<ArrowGenerator>().enabled = true;
        StartCoroutine(WaitArrowFall());
    }

    private IEnumerator WaitArrowFall()
    {
        yield return new WaitForSeconds(1f);

        Player[] players = FindObjectsOfType<Player>();
        foreach (Player p in players)
            p.enabled = true;
    }
}
