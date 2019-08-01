using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour {
    public float shake = 0f;
    public float shakeAmount = 0.7f;
    public float decreaseFactor = 1.0f;

    Vector3 originalPos;


    void OnEnable()
    {
        originalPos = transform.localPosition;
    }

    void Update()
    {
        if (shake > 0)
        {
            transform.localPosition = originalPos + Random.insideUnitSphere * shakeAmount;

            shake -= Time.deltaTime * decreaseFactor;
        }
        else
        {
            shake = 0f;
            transform.localPosition = originalPos;
        }
    }
    
    public void ShakeCam()
    {
        shake = 0.2f;
        shakeAmount = 0.2f;
        decreaseFactor = 2.0f;
    }
}
