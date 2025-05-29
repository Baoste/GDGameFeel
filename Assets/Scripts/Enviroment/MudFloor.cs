using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MudFloor : MonoBehaviour
{
    private Player player;
    private float mudSpeed = 3f;
    // Start is called before the first frame update
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player0") || other.CompareTag("Player1"))
        {
            Player player = other.GetComponentInParent<Player>();
            if (player != null && player.controller != null)
            {
                player.controller.moveSpeed = mudSpeed;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player0") || other.CompareTag("Player1"))
        {
            Player player = other.GetComponentInParent<Player>();
            if (player != null && player.controller != null)
            {
                player.controller.moveSpeed = 9f; 
            }
        }
    }
}
