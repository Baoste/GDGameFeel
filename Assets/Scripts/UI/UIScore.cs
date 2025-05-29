using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIScore : MonoBehaviour
{
    public Image scoreImage;
    public Sprite[] scoreSprites;
    void Start()
    {
        scoreImage = GetComponent<Image>();
        scoreImage.sprite = scoreSprites[0];
    }

}
