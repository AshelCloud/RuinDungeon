using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public float mouseSensitivity = 10f;
    public Transform target;
    public float dstFromTarget = 2f;
    public Vector2 pitchMinMax = new Vector2(-40f, 85f);

    public float rotationSmoothTime = 1.2f;
    Vector3 rotationSmoothVelocity;
    Vector3 currentRoatation;

    float yaw;
    float pitch;

    private bool isUpdate = false;

    public void Initialize(Transform targetTransform)
    {
        target = targetTransform;

        isUpdate = true;
        //플레이어 설정및 System파일에서 Sensitvity, yaw, pitch 가져오셈
    }

    private void LateUpdate()
    {
        if (!isUpdate) { return; }
        if (!Cursor.visible)
        {
            yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
            pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;
            pitch = Mathf.Clamp(pitch, pitchMinMax.x, pitchMinMax.y);

            currentRoatation = Vector3.SmoothDamp(currentRoatation, new Vector3(pitch, yaw), ref rotationSmoothVelocity, rotationSmoothTime);
            transform.eulerAngles = currentRoatation;

            transform.position = target.position - transform.forward * dstFromTarget;

            //Debug.DrawLine(target.position, transform.position, Color.blue);
            RaycastHit CamHitWall;
            Debug.DrawRay(target.position, -transform.forward * 50f, Color.yellow);
            if (Physics.Raycast(target.position, -transform.forward, out CamHitWall, dstFromTarget, (-1) - (1<<9)))
            {
                if (CamHitWall.collider.CompareTag("Wall") || CamHitWall.collider.CompareTag("Ground"))
                {
                    transform.position = new Vector3(CamHitWall.point.x, CamHitWall.point.y, CamHitWall.point.z);
                }
            }
        }
    }

}
