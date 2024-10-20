using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserNodeItem : MonoBehaviour
{
    [HideInInspector] public int m_UniqID = -1;       //������ ������ȣ
    [HideInInspector] public string m_TeamKind = "";  //��
    [HideInInspector] public bool m_IamReady = false; //Ready���� �߰�

    // �̸� ǥ�ÿ� Text UI ������Ʈ
    public Text NameText;
    //Ready ���� ǥ�ÿ� Text UI ������Ʈ
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
