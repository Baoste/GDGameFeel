using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireArea : MonoBehaviour
{
    public float consistTime = 5f;
    void Start()
    {
        transform.localScale = Vector3.zero;
        transform.DOScale(Vector3.one * 12f, 1.5f)
        .SetEase(Ease.InCubic)
        .OnComplete(() =>
        {
            transform.DOScale(Vector3.zero, 1.5f)
            .SetDelay(consistTime)
            .SetEase(Ease.InCubic)
            .OnComplete(() => {
                Destroy(gameObject, .5f);
            });
        });
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.gameObject.GetComponent<Player>();
        if (player != null)
        {
            player.controller.moveSpeed = 5f;
            player.dashCoolTime = 9f;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Player player = collision.gameObject.GetComponent<Player>();
        if (player != null)
        {
            player.controller.moveSpeed = 9f;
            player.dashCoolTime = 0f;
        }
    }
}
