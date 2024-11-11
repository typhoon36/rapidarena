using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayUserId : MonoBehaviour
{
    public Text userId;
    PhotonView pv = null;


    void Start()
    {
        pv = GetComponent<PhotonView>();
        userId.text = pv.Owner.NickName;
    }

    public void ChangeNameColor(Ready_Mgr a_ReadyMgr)
    {
        if (pv == null) return;

        if (a_ReadyMgr == null) return;

        string a_TeamKind = a_ReadyMgr.ReceiveSelTeam(pv.Owner);

        if (a_TeamKind == "blue")
            userId.color = HexColor("#4179A3"); // �Ķ���
        else if (a_TeamKind == "red")
            userId.color = HexColor("#DC626D"); // ������
    }

    //�����ڵ带 ����ڵ�� ��ȯ
    Color HexColor(string hex)
    {
        if (ColorUtility.TryParseHtmlString(hex, out Color color))
        {
            return color;
        }
        return Color.white; 
    }
}
