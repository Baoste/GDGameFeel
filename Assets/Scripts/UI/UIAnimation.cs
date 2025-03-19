using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIAnimation : MonoBehaviour
{
    public Sprite[] sprites;

    private Image image;
    private int imgIndex;
    private int imgCount;
    private float time;

    private void Start()
    {
        image = GetComponent<Image>();
        time = 0;
        imgIndex = 0;
        imgCount = sprites.Length;
    }
    private void Update()
    {
        time += Time.deltaTime;
        if (time >= .3f)
        {
            time = 0;
            image.sprite = sprites[imgIndex];
            imgIndex = (imgIndex + 1) % imgCount;
        }
    }

}
