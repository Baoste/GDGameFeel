using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeverTrigger : MonoBehaviour
{
    public MovingPlatform platform;
    public Sprite triggeredSprite;
    private SpriteRenderer spriteRenderer;
    public bool isTriggered = false;
    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Arrow arrow = other.GetComponent<Arrow>();
        if (!isTriggered && (other.CompareTag("PlayerFoot") || arrow != null))
        {
            isTriggered = true;
            spriteRenderer.sprite = triggeredSprite;
            platform.StartMoving();
        }
    }
}
