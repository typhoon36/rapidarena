using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserNodeItem : MonoBehaviour
{
    [HideInInspector] public int m_UniqID = -1;       //유저의 고유번호
    [HideInInspector] public string m_TeamKind = "";  //팀
    [HideInInspector] public bool m_IamReady = false; //Ready상태 추가

    // 이름 표시용 Text UI 컴포넌트
    public Text NameText;
    //Ready 상태 표시용 Text UI 컴포넌트
    public Text StateInfoText;

    public void DispPlayerData(string a_TankName, bool isMine = false)
    {
        if (isMine == true)
        {
            NameText.color = Color.gray;
            NameText.text = a_TankName;
        }
        else
            NameText.text = a_TankName;

        if (m_IamReady == true)
            StateInfoText.text = "<color=#ff0000>Ready</color>";
        else
            StateInfoText.text = "";
    }
}
