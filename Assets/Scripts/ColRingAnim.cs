using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SocialPlatforms;

public class ColRingAnim : MonoBehaviour
{
    public GameObject ring;
    public GameObject tail;
    
    private Light2D ringLight;
    private Light2D tailLight;

    private float timeScale;
    private float amount;
    
    void Start()
    {
        amount = 1.8f;
        ringLight = ring.GetComponent<Light2D>();
        tailLight = tail.GetComponent<Light2D>();
    }

    void Update()
    {
        amount -= 3.3f * Time.deltaTime;
        timeScale = Mathf.Pow(2.5f, amount);
        Debug.Log(timeScale);

        transform.Rotate(Vector3.forward, timeScale * 30f * Time.deltaTime);

        ringLight.intensity -= timeScale * 10f * Time.deltaTime;
        tailLight.intensity -= timeScale * 10f * Time.deltaTime;
        
        ring.transform.localScale += timeScale * Vector3.one * 2f * Time.deltaTime;

        Vector3 scale = tail.transform.localScale;
        scale.x -= timeScale * .7f * Time.deltaTime;
        scale.y -= timeScale * 2.3f * Time.deltaTime;
        tail.transform.localScale = scale;

        if (ringLight.intensity <= 0)
        {
            Destroy(gameObject);
        }
    }
}
