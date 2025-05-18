using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WinnerUIAnim : MonoBehaviour
{
    private float animTime;
    private int moveCount;
    private float moveScale;
    private Image image;

    public Sprite[] winners;
    public Player[] players;
    public CinemachineVirtualCamera virtualCamera;

    void Start()
    {
        animTime = 0;
        moveCount = 0;
        moveScale = 0.2f;
    }

    void Update()
    {
        animTime += Time.deltaTime;
        if (animTime > 1f)
        {
            animTime = 0;
            moveCount++;

            if (moveCount > 3)
                transform.position += Vector3.up * moveScale;
            else
                transform.position += Vector3.down * moveScale;


            if (moveCount > 5)
                moveCount = 0;
        }
    }

    public void ChangeWinnerSprite(int index)
    {
        image = GetComponent<Image>();
        if (image.sprite == null)
        {
            image.sprite = winners[index];
            virtualCamera.Follow = players[index].transform;
            virtualCamera.m_Lens.OrthographicSize = 2;
        }
    }

    public void ReturnCam()
    {
        virtualCamera.Follow = GameObject.Find("PlayerGroup").transform;
        virtualCamera.m_Lens.OrthographicSize = 1;
    }
}
