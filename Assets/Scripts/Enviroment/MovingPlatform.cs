using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public Transform targetPoint;
    public LeverTrigger trigger;
    public Sprite untriggeredSprite;
    public float moveSpeed = 2f;
    private bool isMoving = false;

    public void StartMoving()
    {
        isMoving = true;
    }

    private void Update()
    {
        if (isMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPoint.position, moveSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, targetPoint.position) < 0.01f)
            {
                isMoving = false;
                trigger.GetComponent<SpriteRenderer>().sprite = untriggeredSprite;
            }
        }
    }
}
