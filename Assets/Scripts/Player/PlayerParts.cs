using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerParts : MonoBehaviour
{
    private Collider2D[] cols;
    private Rigidbody2D[] rbs;
    private SpriteRenderer[] sps;
    private void Start()
    {
        cols = GetComponentsInChildren<Collider2D>();
        rbs = GetComponentsInChildren<Rigidbody2D>();
        sps = GetComponentsInChildren<SpriteRenderer>();
    }
    public void DeadTrigger()
    {
        foreach (var col in cols)
        {
            col.isTrigger = false;
        }
        foreach (var rb in rbs)
        {
            rb.simulated = true;
            Vector2 dir = new Vector2(Random.Range(0f, 1f), Random.Range(0f, 1f));
            rb.AddForce(dir.normalized * 15f, ForceMode2D.Impulse);
        }
        foreach (var sp in sps)
        {
            sp.enabled = true;
        }
    }
}
