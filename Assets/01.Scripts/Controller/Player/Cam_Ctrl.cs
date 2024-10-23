using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//FPS 카메라 컨트롤 스크립트
public class Cam_Ctrl : MonoBehaviour
{
    private Transform playerTransform;
    private float rotationSpeed = 5.0f;
    private float verticalRotation = 0.0f;

    public void Init(GameObject player)
    {
        playerTransform = player.transform;
    }

    public void UpdateRot(float mouseX, float mouseY)
    {
        // 플레이어의 Y축 회전
        playerTransform.Rotate(0, mouseX * rotationSpeed, 0);

        // 카메라의 X축 회전 (상하 회전)
        verticalRotation -= mouseY * rotationSpeed;
        verticalRotation = Mathf.Clamp(verticalRotation, -10f, 10f);

        transform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
    }

    void LateUpdate()
    {
        // 카메라의 회전을 플레이어의 회전과 동기화
        transform.rotation = Quaternion.Euler(verticalRotation, playerTransform.eulerAngles.y, 0);
    }
}
