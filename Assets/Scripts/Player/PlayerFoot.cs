using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerFoot : MonoBehaviour
{
    public Tilemap tilemap;
    private Player player;
    private void Start()
    {
        player = GetComponentInParent<Player>();
    }

    private void Update()
    {
        Vector3Int cell = tilemap.WorldToCell(player.transform.position);
        if (tilemap.GetTile(cell) == null)
            player.stateMachine.ChangeState(player.fallState);
    }
}
