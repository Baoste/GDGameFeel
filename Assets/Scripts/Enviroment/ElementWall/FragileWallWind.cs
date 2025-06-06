using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FragileWallWind : FragileWall, IElementWind
{
    public GameObject tornadoPrefab;
    public GameObject fireArea;

    protected override void HandleCollision(Collision2D collision)
    {
        // base.HandleCollision(collision);

        IElementWater isWater = collision.gameObject.GetComponent<IElementWater>();
        if (isWater != null)
        {
            Arrow arrow = collision.gameObject.GetComponent<Arrow>();
            if (!arrow.IsElementalEffectTriggered && !IsElementalEffectTriggered)
            {
                arrow.ElementalEffectTriggered();
                // ElementalEffectTriggered();
                Instantiate(tornadoPrefab, collision.contacts[0].point, Quaternion.identity);
            }
        }

        IElementFire isFire = collision.gameObject.GetComponent<IElementFire>();
        if (isFire != null)
        {
            Arrow arrow = collision.gameObject.GetComponent<Arrow>();
            if (!arrow.IsElementalEffectTriggered && !IsElementalEffectTriggered)
            {
                arrow.ElementalEffectTriggered();
                Instantiate(fireArea, collision.contacts[0].point, Quaternion.identity);
            }
        }
    }
}
