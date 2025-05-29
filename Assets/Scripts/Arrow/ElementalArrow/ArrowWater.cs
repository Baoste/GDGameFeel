using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowWater : Arrow, IElementWater
{
    public GameObject fogPrefab;
    public ParticleSystem effectParticle;

    public override bool ElementalEffectTriggered()
    {
        effectParticle.Stop();
        return base.ElementalEffectTriggered();
    }

    protected override void HandleCollision(Collision2D collision)
    {
        base.HandleCollision(collision);

        IElementFire isFire = collision.gameObject.GetComponent<IElementFire>();
        if (isFire != null)
        {
            Arrow arrow = collision.gameObject.GetComponent<Arrow>();
            if (!arrow.IsElementalEffectTriggered && !IsElementalEffectTriggered)
            {
                arrow.ElementalEffectTriggered();
                ElementalEffectTriggered();
                Instantiate(fogPrefab, transform.position, Quaternion.identity);
            }
        }
    }
}
