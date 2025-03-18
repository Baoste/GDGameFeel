using System;
using System.Collections;
using UnityEngine;

public class WaveGenerator : MonoBehaviour
{
    [SerializeField] private float shockWaveTime = 0.75f;
    private Coroutine waveCoroutine;
    private Material material;
    private static int wavDistFromCenter = Shader.PropertyToID("_WaveDistFromCenter");

    private void Awake()
    {
        material = GetComponent<SpriteRenderer>().material;
    }
    public void CallShockWave()
    {
        waveCoroutine = StartCoroutine(ShockWaveAction(-0.1f, 1f));
    }

    private IEnumerator ShockWaveAction(float startPos, float endPos)
    {
        material.SetFloat(wavDistFromCenter, startPos);
        float lerpAmount = 0f;
        float elapsedTime = 0f;
        while (elapsedTime < shockWaveTime)
        {
            elapsedTime += Time.deltaTime * (MathF.Exp(-elapsedTime) + 1);
            lerpAmount = Mathf.Lerp(startPos, endPos, elapsedTime / shockWaveTime);
            material.SetFloat(wavDistFromCenter, lerpAmount);
            yield return null;
        }
        material.SetFloat(wavDistFromCenter, -0.1f);
    }
}
