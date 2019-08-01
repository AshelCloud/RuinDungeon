using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpItemText : MonoBehaviour
{
    private float rotationSpeed;

    TextMesh text;
    private void Start()
    {
        text = GetComponent<TextMesh>();
        gameObject.SetActive(false);
        rotationSpeed = 100f;
    }

    //Camera 방향맞춰서 빙빙 돌려욧 >~<
    private void Update()
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(transform.position - Camera.main.transform.position), rotationSpeed * Time.deltaTime);
    }
}
