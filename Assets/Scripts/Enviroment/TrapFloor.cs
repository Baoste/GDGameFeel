using System.Collections;
using UnityEngine;

public class TrapFloor : MonoBehaviour
{
    //public float disappearDelay = 0.1f;
    public float wobbleDuration = 0.5f;
    public float sinkDistance = 0.5f;
    public float sinkDuration = 0.5f;
    public float respawnTime = 3f;

    private SpriteRenderer spriteRenderer;
    private Collider2D floorCollider;
    private Vector3 originalPosition;
    private Vector3 originalScale;
    private bool isTriggered = false;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        floorCollider = GetComponent<Collider2D>();
        originalScale = transform.localScale;
        originalPosition = transform.position;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isTriggered && other.CompareTag("PlayerFoot"))
        {
            StartCoroutine(HandleTrap());
        }
    }

    IEnumerator HandleTrap()
    {
        isTriggered = true;
        float elapsed = 0f;
        while (elapsed < wobbleDuration)
        {
            float offsetX = Mathf.Sin(elapsed * 40f) * 0.05f;
            transform.position = originalPosition + new Vector3(offsetX, 0, 0);
            elapsed += Time.deltaTime;
            yield return null;
        }

        Vector3 targetPos = originalPosition - new Vector3(0f, sinkDistance, 0f);
        Vector3 targetScale = originalScale * 0.1f; 
        elapsed = 0f;
        while (elapsed < sinkDuration)
        {
            float t = elapsed / sinkDuration;
            transform.position = Vector3.Lerp(originalPosition, targetPos, t);
            transform.localScale = Vector3.Lerp(originalScale, targetScale, t); 
            elapsed += Time.deltaTime;
            yield return null;
        }

        // 消失
        spriteRenderer.enabled = false;
        floorCollider.enabled = false;

        yield return new WaitForSeconds(respawnTime);

        // 重新出现
        transform.position = originalPosition;
        transform.localScale = originalScale;
        spriteRenderer.enabled = true;
        floorCollider.enabled = true;
        isTriggered = false;
    }
}
