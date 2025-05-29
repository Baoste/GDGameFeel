using Cinemachine;
using DG.Tweening;
using UnityEngine;

public class ScorePart : MonoBehaviour
{
    private ParticleSystem particles;
    private SpriteRenderer sprite;
    public Color color;

    private void Start()
    {
        particles = GetComponent<ParticleSystem>();
        sprite = GetComponent<SpriteRenderer>();
        sprite.color = color;
        transform.localScale = Vector3.zero;
    }

    public void MoveToTarget(CinemachineImpulseSource impulseSource, float impulseForce, Vector3 target)
    {
        Vector2 randomDir = Random.insideUnitCircle.normalized;
        Vector3 targetPos = new Vector3(randomDir.x, randomDir.y, 0f) * Random.Range(0.5f, 1f);
        transform.DOLocalMove(targetPos, 0.5f).SetEase(Ease.InCubic);

        transform.DOScale(Vector3.one * Random.Range(0.7f, 1f), 0.5f).SetEase(Ease.InCubic)
        .OnComplete(() =>
        {
            transform.DOMove(target, 0.5f)
            .SetEase(Ease.InCubic)
            .OnComplete(() =>
            {
                impulseSource.m_DefaultVelocity = Vector2.one * impulseForce;
                impulseSource.GenerateImpulse();
                sprite.color = Vector4.zero;
                particles.Play();
                Destroy(gameObject, 1f);
            });
        });
    }
}
