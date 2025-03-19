
using UnityEngine;
using UnityEngine.Tilemaps;

public class FragileWall : MonoBehaviour, IFragile
{
    private Tilemap tilemap;
    private ParticleGenerator particleGenerator;

    void Start()
    {
        tilemap = GetComponent<Tilemap>();
        particleGenerator = FindObjectOfType<ParticleGenerator>();
    }

    public bool Destroy(Vector3 pos)
    {
        Vector3Int cell = tilemap.WorldToCell(pos);
        if (tilemap.GetTile(cell) == null)
            return false;
        tilemap.SetTile(cell, null);
        particleGenerator.GenerateParticle(pos);
        return true;
    }
}
