using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//FPS 카메라를 위한 컨트롤러
public class Cam_Ctrl : MonoBehaviour
{
    GameObject m_Player;

    #region CamMove
    [SerializeField]
    private float m_CamRotXSpeed = 5f;
    [SerializeField]
    private float m_CamRotYSpeed = 3f;

    float limitMinX = -80f;
    float limitMaxX = 50f;
    float eulerAngleX;
    float eulerAngleY;
    #endregion

    public void Init(GameObject a_Player)
    {
        m_Player = a_Player;
    }

    public void UpdateRot(float a_MouseX, float a_MouseY)
    {
        eulerAngleY += a_MouseX * m_CamRotXSpeed;
        eulerAngleX -= a_MouseY * m_CamRotYSpeed;

        //X축회전시 제한을 위한 함수 호출
        eulerAngleX = ClampAngle(eulerAngleX, limitMinX, limitMaxX);

        transform.rotation = Quaternion.Euler(eulerAngleX, eulerAngleY, 0);
    }

    float ClampAngle(float a_Angle, float a_Min, float a_Max)
    {
        if (a_Angle < -360) a_Angle += 360;
        if (a_Angle > 360) a_Angle -= 360;

        return Mathf.Clamp(a_Angle, a_Min, a_Max);
    }


}
