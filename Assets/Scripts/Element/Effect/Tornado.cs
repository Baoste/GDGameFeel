using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tornado : MonoBehaviour
{
    public float consistTime = 5f;
    void Start()
    {
        transform.localScale = Vector3.zero;
        transform.DOScale(Vector3.one * 12f, .5f)
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

}
