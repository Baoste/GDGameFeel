
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Tilemaps;

public class FragileWall : MonoBehaviour, IFragile
{
    private Tilemap tilemap;
    private ParticleGenerator particleGenerator;
    public bool IsElementalEffectTriggered { get; set; } = false;

    protected void Start()
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

    protected void OnCollisionEnter2D(Collision2D collision)
    {
        HandleCollision(collision);
    }

    protected virtual void HandleCollision(Collision2D collision)
    {
    }

    public bool ElementalEffectTriggered()
    {
        bool tmp = IsElementalEffectTriggered;
        if (!IsElementalEffectTriggered)
            IsElementalEffectTriggered = true;
        return tmp;
    }
}
