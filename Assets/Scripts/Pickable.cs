
using UnityEngine;

public class Pickable : MonoBehaviour
{
    private Arrow arrow;
    private AudioManager audioManager;
    private void Start()
    {
        arrow = GetComponentInParent<Arrow>();
        audioManager = FindObjectOfType<AudioManager>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision != null)
        {
            Player player = collision.gameObject.GetComponent<Player>();
            if (player?.stateMachine.currentState != player?.deadState)
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
