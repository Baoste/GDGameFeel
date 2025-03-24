using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowDestroy : MonoBehaviour
{
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision)
        {
            Arrow arrow = collision.GetComponent<Arrow>();
            if (arrow)
                Destroy(arrow.gameObject);
        }
    }
}
