using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShadow : MonoBehaviour
{
    private SpriteRenderer sp;
    private Color color;
    void Start()
    {
        sp = GetComponent<SpriteRenderer>();
        color = sp.color;
    }

    // Update is called once per frame
    void Update()
    {
        color.a -= Time.deltaTime;
        sp.color = color;
        if (color.a <= 0f)
        {
            Destroy(gameObject);
        }
    }
}
