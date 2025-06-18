using Cinemachine;
using DG.Tweening;
using UnityEngine;

public class ScorePart : MonoBehaviour
{
    private ParticleSystem particles;
    private SpriteRenderer sprite;
    private AudioManager audioManager;
    public Color color;

    private void Start()
    {
        particles = GetComponent<ParticleSystem>();
        sprite = GetComponent<SpriteRenderer>();
        audioManager = FindObjectOfType<AudioManager>();

        sprite.color = color;
        transform.localScale = Vector3.zero;
    }

    public void MoveToTarget(CinemachineImpulseSource impulseSource, float impulseForce, GameObject targetPos, Color targetColor)
    {
        Vector2 randomDir = Random.insideUnitCircle.normalized;
        Vector3 tmpPos = transform.position + new Vector3(randomDir.x, randomDir.y, 0f) * Random.Range(0.5f, 1f);
        transform.DOLocalMove(tmpPos, 0.5f).SetEase(Ease.InCubic);

        transform.DOScale(Vector3.one * Random.Range(0.7f, 1f), 0.5f)
        .SetEase(Ease.InCubic)
        .OnComplete(() =>
        {
            sprite.DOColor(targetColor, 0.5f).SetEase(Ease.InCubic);
            audioManager.PlaySfx(audioManager.platMove);
            transform.DOMove(targetPos.transform.position, 0.5f)
            .SetEase(Ease.InCubic)
            .OnComplete(() =>
            {
                impulseSource.m_DefaultVelocity = Vector2.one * impulseForce;
                impulseSource.GenerateImpulse();
                sprite.color = Vector4.zero;
                var main = particles.main;
                main.startColor = targetColor;
                particles.Play();
                Destroy(gameObject, 1f);
            });
        });
    }
}
