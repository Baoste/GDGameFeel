using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FragileWall : MonoBehaviour, IFragile
{
    private Tilemap tilemap;
    void Start()
    {
        tilemap = GetComponent<Tilemap>();
    }

    public void Destroy(Vector3 pos)
    {
        Vector3Int cell = tilemap.WorldToCell(pos);
        tilemap.SetTile(cell, null);
    }
}
