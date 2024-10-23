using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//FPS ī�޶� ��Ʈ�� ��ũ��Ʈ
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
        // �÷��̾��� Y�� ȸ��
        playerTransform.Rotate(0, mouseX * rotationSpeed, 0);

        // ī�޶��� X�� ȸ�� (���� ȸ��)
        verticalRotation -= mouseY * rotationSpeed;
        verticalRotation = Mathf.Clamp(verticalRotation, -10f, 10f);

        transform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
    }

    void LateUpdate()
    {
        // ī�޶��� ȸ���� �÷��̾��� ȸ���� ����ȭ
        transform.rotation = Quaternion.Euler(verticalRotation, playerTransform.eulerAngles.y, 0);
    }
}
