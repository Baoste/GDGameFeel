using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FragileWallWater : FragileWall, IElementWater
{
    public GameObject fogPrefab;
    public GameObject tornadoPrefab;

    protected override void HandleCollision(Collision2D collision)
    {
        // base.HandleCollision(collision);

        IElementFire isFire = collision.gameObject.GetComponent<IElementFire>();
        if (isFire != null)
        {
            Arrow arrow = collision.gameObject.GetComponent<Arrow>();
            if (!arrow.IsElementalEffectTriggered)
            {
                arrow.ElementalEffectTriggered();
                // ElementalEffectTriggered();
                Instantiate(fogPrefab, collision.contacts[0].point, Quaternion.identity);
            }
        }

        IElementWind isWind = collision.gameObject.GetComponent<IElementWind>();
        if (isWind != null)
        {
            Arrow arrow = collision.gameObject.GetComponent<Arrow>();
            if (!arrow.IsElementalEffectTriggered)
            {
                arrow.ElementalEffectTriggered();
                Instantiate(tornadoPrefab, collision.contacts[0].point, Quaternion.identity);
            }
        }
    }
}
