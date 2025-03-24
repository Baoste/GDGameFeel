using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerFoot : MonoBehaviour
{
    public Tilemap tilemap;
    private Player player;
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
            player.stateMachine.ChangeState(player.fallState);
            isFall = true;
        }
    }
}
