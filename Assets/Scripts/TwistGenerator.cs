using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwistGenerator : MonoBehaviour
{
    [SerializeField] private float shockTwistTime = 1.5f;
    private Coroutine twistCoroutine;
    private Material material;
    private static int size = Shader.PropertyToID("_Size");

    private void Awake()
    {
        material = GetComponent<SpriteRenderer>().material;
    }
    public void CallShockTwist()
    {
        if (twistCoroutine != null)
            StopCoroutine(twistCoroutine);
        twistCoroutine = StartCoroutine(ShockTwistAction(0f, 0.2f));
    }

    private IEnumerator ShockTwistAction(float startPos, float endPos)
    {
        material.SetFloat(size, endPos);
        float lerpAmount = 0f;
        float elapsedTime = 0f;
        while (elapsedTime < shockTwistTime)
        {
            elapsedTime += Time.deltaTime * (MathF.Exp(-elapsedTime) + 1);
            lerpAmount = Mathf.Lerp(endPos, startPos, elapsedTime / shockTwistTime);
            material.SetFloat(size, lerpAmount);
            yield return null;
        }
        
        material.SetFloat(size, 0f);
    }
}
