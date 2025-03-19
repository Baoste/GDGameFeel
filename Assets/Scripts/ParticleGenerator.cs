using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleGenerator : MonoBehaviour
{
    public GameObject particle;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void GenerateParticle(Vector3 pos)
    {
        Instantiate(particle, pos, Quaternion.identity);
    }
}
