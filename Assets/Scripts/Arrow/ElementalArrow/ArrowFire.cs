using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowFire : Arrow, IElementFire
{
    public GameObject fireArea;
    public ParticleSystem effectParticle;

    public override bool ElementalEffectTriggered()
    {
        effectParticle.Stop();
        return base.ElementalEffectTriggered();
    }

    protected override void HandleCollision(Collision2D collision)
    {
        base.HandleCollision(collision);

        IElementWind isWind = collision.gameObject.GetComponent<IElementWind>();
        if (isWind != null)
        {
            Arrow arrow = collision.gameObject.GetComponent<Arrow>();
            if (!arrow.IsElementalEffectTriggered && !IsElementalEffectTriggered)
            {
                arrow.ElementalEffectTriggered();
                ElementalEffectTriggered();
                Instantiate(fireArea, transform.position, Quaternion.identity);
            }
        }
    }
}
