using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerFoot : MonoBehaviour
{
    public Tilemap tilemap;
    public LayerMask floorLayer;
    private Player player;
    private Collider2D trapFloorCollider;
    private bool isFall;
    private void Start()
    {
        player = GetComponentInParent<Player>();
        isFall = false;
    }

    private void Update()
    {
        Vector3Int cell = tilemap.WorldToCell(player.transform.position);

        if (tilemap.GetTile(cell) == null && !isFall)
        {
            Vector2 footPos = player.GetComponent<Collider2D>().bounds.min;
            Collider2D groundCheck = Physics2D.OverlapBox(footPos, new Vector2(0.8f, 0.1f), 0f, floorLayer);
            Debug.Log(groundCheck);
            if (groundCheck == null && player.stateMachine.currentState != player.dashingState
                && player.stateMachine.currentState != player.dashInState
                && player.stateMachine.currentState != player.dashOutState)
            {
                player.stateMachine.ChangeState(player.fallState);
                isFall = true;
            }
        }
    }
}
