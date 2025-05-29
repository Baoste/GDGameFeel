using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowWind : Arrow, IElementWind
{
    public GameObject tornadoPrefab;
    public ParticleSystem effectParticle;

    public override bool ElementalEffectTriggered()
    {
        effectParticle.Stop();
        return base.ElementalEffectTriggered();
    }

    protected override void HandleCollision(Collision2D collision)
    {
        base.HandleCollision(collision);

        IElementWater isWater = collision.gameObject.GetComponent<IElementWater>();
        if (isWater != null)
        {
            Arrow arrow = collision.gameObject.GetComponent<Arrow>();
            if (!arrow.IsElementalEffectTriggered && !IsElementalEffectTriggered)
            {
                arrow.ElementalEffectTriggered();
                ElementalEffectTriggered();
                Instantiate(tornadoPrefab, transform.position, Quaternion.identity);
            }
        }
    }
}
