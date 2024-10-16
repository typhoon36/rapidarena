using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomItem : MonoBehaviour
{
    //�ܺ� ������ ���� public ������
    [HideInInspector] public string roomName = "";
    [HideInInspector] public int connectPlayer = 0;
    [HideInInspector] public int maxPlayer = 0;

    //�� �̸� ǥ���� Text UI �׸�
    public Text textRoomName;
    //�� ������ ���� �ִ� ������ ���� ǥ���� Text UI �׸�
    public Text textConnectInfo;

    [HideInInspector] public string ReadyState = ""; //���� ���� ǥ��

    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<Button>().onClick.AddListener(() =>
        {
            Lobby_Mgr refPtInit = FindObjectOfType<Lobby_Mgr>();
            if(refPtInit != null )
               refPtInit.OnClickRoomItem(roomName);
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DispRoomData(bool a_IsOpen)
    {
        if(a_IsOpen == true)
        {
            textRoomName.color = new Color32(0, 0, 0, 255);
            textConnectInfo.color = new Color32(0, 0, 0, 255);
        }
        else
        {
            textRoomName.color = new Color32(0, 0, 255, 255);
            textConnectInfo.color = new Color32(0, 0, 255, 255);
        }

        textRoomName.text = roomName;
        textConnectInfo.text = "(" + connectPlayer.ToString() + "/"
                                   + maxPlayer.ToString() + ")";
    }//public void DispRoomData(bool a_IsOpen)
}
