using Cinemachine;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public LeverTrigger trigger;
    public Sprite untriggeredSprite;
    public float moveSpeed = 2f;
    private bool isMoving = false;

    private MapMarker mapMarker;
    private List<Vector3> targetPoints;
    private int movingIndex = 0;    // Index of the current target point
    private int movingDirction = 1;

    private AudioManager audioManager;
    private CinemachineImpulseSource impulseSource;

    private void Start()
    {
        impulseSource = GetComponent<CinemachineImpulseSource>();
        audioManager = FindObjectOfType<AudioManager>();
        mapMarker = GetComponent<MapMarker>();
        targetPoints = new List<Vector3>();
        foreach (var pos in mapMarker.markerPositions)
        {
            targetPoints.Add(transform.position + pos);
        }
    }

    public void StartMoving()
    {
        if (!isMoving)
        {
            if (movingIndex >= targetPoints.Count - 1)
            {
                movingDirction = -1; // Change direction to reverse
            }
            else if (movingIndex <= 0)
            {
                movingDirction = 1; // Change direction to forward
            }
            movingIndex = movingIndex + movingDirction;

            audioManager.PlaySfx(audioManager.platMove);
            transform.DOMove(targetPoints[movingIndex], moveSpeed)
            .SetEase(Ease.InExpo)
            .OnComplete(() =>
            {
                audioManager.PlaySfx(audioManager.floorHit);
                impulseSource.GenerateImpulse();
                isMoving = false;
                trigger.isTriggered = false;
                trigger.GetComponent<SpriteRenderer>().sprite = untriggeredSprite;
            });
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerFoot"))
        {
            Player player = collision.GetComponentInParent<Player>();
            player.transform.parent = transform;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerFoot"))
        {
            Player player = collision.GetComponentInParent<Player>();
            player.transform.parent = transform.parent.parent;
        }
    }
}
