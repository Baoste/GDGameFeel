using Cinemachine;
using DG.Tweening;
using System.Collections;
using UnityEngine;

public class ScorePartGenrator : MonoBehaviour
{
    public GameObject scorePartPrefab;
    private Player player;
    private int count = 10;
    private Color playerColor;
    private AudioManager audioManager;
    public CinemachineImpulseSource impulseSource { get; private set; }

    void Start()
    {
        player = GetComponentInParent<Player>();
        impulseSource = GetComponent<CinemachineImpulseSource>();
        audioManager = FindObjectOfType<AudioManager>();
        // Á£×ÓÑÕÉ«
        playerColor = player.playerColor;
    }

    public void GenerateScoreParts(Vector3 pos)
    {
        StartCoroutine(GenerateScorePartsCoroutine(pos));
    }

    private IEnumerator GenerateScorePartsCoroutine(Vector3 pos)
    {
        float impulseForce = 0.02f;
        float delay = 0.03f;
        // audioManager.PlaySfx(audioManager.generateScore);

        for (int i = 0; i < count; i++)
        {
            ScorePart scorePart = Instantiate(scorePartPrefab, pos, Quaternion.identity).GetComponent<ScorePart>();
            scorePart.color = playerColor;
            Vector3 targetPos = player.enemyScore.gameObject.transform.position;
            Color targetColor = player.enemyColor;
            scorePart.MoveToTarget(impulseSource, impulseForce, player.enemyScore.gameObject, targetColor);
            impulseForce *= 1.2f;

            yield return new WaitForSeconds(delay);
        }

        yield return new WaitForSeconds(1f);
        player.deathCount.Value += 1;
        audioManager.PlaySfx(audioManager.addScore);
    }
}
