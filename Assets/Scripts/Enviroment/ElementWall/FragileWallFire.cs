using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FragileWallFire : FragileWall, IElementFire
{
    public GameObject fogPrefab;
    public GameObject fireArea;

    protected override void HandleCollision(Collision2D collision)
    {
        // base.HandleCollision(collision);

        IElementWater isWater = collision.gameObject.GetComponent<IElementWater>();
        if (isWater != null)
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
                Instantiate(fireArea, collision.contacts[0].point, Quaternion.identity);
            }
        }
    }
}
