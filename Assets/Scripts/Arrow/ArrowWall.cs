using DG.Tweening;
using System.Collections;
using UnityEngine;

public class ArrowWall : MonoBehaviour
{
    private SpriteRenderer sp;
    private bool isHit;
    private float sign;
    private int hitTime;
    private void Start()
    {
        sp = GetComponent<SpriteRenderer>();
        isHit = false;
        sign = 1;
        hitTime = 0;
    }
    private void Update()
    {
        if (isHit)
        {
            Color color = sp.color;
            color.a += sign * Time.deltaTime * 5f;
            if (color.a < 0 || color.a > 1)
            {
                hitTime++;
                sign = -sign;
            }
            if (hitTime >= 4)
            {
                sign = 1;
                hitTime = 0;
                isHit = false;
                color.a = 0;
            }
            sp.color = color;
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision != null)
        {
            isHit = true;
        }
    }

}
