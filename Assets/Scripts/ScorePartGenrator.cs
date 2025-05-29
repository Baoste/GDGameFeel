using Cinemachine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScorePartGenrator : MonoBehaviour
{
    public GameObject scorePartPrefab;
    private Player player;
    private int count = 10;
    private Color playerColor;
    public CinemachineImpulseSource impulseSource { get; private set; }

    void Start()
    {
        player = GetComponentInParent<Player>();
        impulseSource = GetComponent<CinemachineImpulseSource>();
        // Á£×ÓÑÕÉ«
        //playerColor = player.GetComponentInChildren<SpriteRenderer>().color;
        playerColor = Color.white;
    }

    public void GenerateScoreParts(Transform trans)
    {
        StartCoroutine(GenerateScorePartsCoroutine(trans));
    }

    private IEnumerator GenerateScorePartsCoroutine(Transform trans)
    {
        float impulseForce = 0.01f;
        float delay = 0.03f;

        for (int i = 0; i < count; i++)
        {
            ScorePart scorePart = Instantiate(scorePartPrefab, trans).GetComponent<ScorePart>();
            scorePart.color = playerColor;
            Vector3 target = player.enemyScore.gameObject.transform.position;
            scorePart.MoveToTarget(impulseSource, impulseForce, target);
            impulseForce *= 1.2f;

            yield return new WaitForSeconds(delay);
        }

        player.deathCount.Value = player.deathCount.Value + 1;
    }
}
