
using UnityEngine;

public class Pickable : MonoBehaviour
{
    private Arrow arrow;
    private void Start()
    {
        arrow = GetComponentInParent<Arrow>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision != null)
        {
            arrow.getArrowPlayer = collision.gameObject.GetComponent<Player>();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision != null)
        {
            if (collision.gameObject.GetComponent<Player>() != null)
                arrow.getArrowPlayer = null;
        }
    }
}
