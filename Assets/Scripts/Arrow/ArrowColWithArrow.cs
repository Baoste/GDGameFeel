using System.Collections;
using UnityEngine;

public class ArrowColWithArrow : MonoBehaviour
{
    private Arrow arrow;

    private void Start()
    {
        arrow = GetComponentInParent<Arrow>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // arrow coliider
        if (arrow != null)
        {
            arrow.audioManager.PlaySfx(arrow.audioManager.hitArrow);
            arrow.impulseSource.GenerateImpulse();
            ContactPoint2D contact = collision.contacts[0];
            Vector3 pos = contact.point;
            //Instantiate(hitRing, pos, Quaternion.identity);
            float angle = Random.Range(0f, 20f);
            ColRingAnim ring = Instantiate(arrow.hitRing, pos, Quaternion.Euler(new Vector3(0, 0, angle))).GetComponent<ColRingAnim>();
            ring.intensity = 8f + arrow.lerpAmount * 8f;
            arrow.waveGenerator.transform.position = pos;
            arrow.waveGenerator.CallShockWave();
            StartCoroutine(WaitToStop(.1f));
        }
    }

    private IEnumerator WaitToStop(float t)
    {
        yield return new WaitForSeconds(t);
        if (arrow.stateMachine.currentState == arrow.fallState)
        {
            arrow.GetComponent<SpriteRenderer>().sprite = arrow.normal;
            arrow.arrowLight.lightCookieSprite = arrow.normal;
            arrow.arrowLight.intensity = 0f;
        }
        arrow.stateMachine.ChangeState(arrow.stopState);
    }
}
