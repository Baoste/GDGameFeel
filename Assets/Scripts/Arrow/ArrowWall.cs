using DG.Tweening;
using UnityEngine;

public class ArrowWall : MonoBehaviour
{
    private SpriteRenderer sp;
    private void Start()
    {
        sp = GetComponent<SpriteRenderer>();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision != null)
        {
            Arrow arrow = collision.gameObject.GetComponent<Arrow>();
            if (arrow != null)
            {
                sp.DOFade(1f, 0.2f)
                    .SetLoops(2, LoopType.Yoyo)
                    .SetEase(Ease.InCubic);
            }
        }
    }
}
