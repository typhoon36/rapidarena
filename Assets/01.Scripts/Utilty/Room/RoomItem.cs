using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomItem : MonoBehaviour
{
    
    [HideInInspector] public string roomName = "";
    [HideInInspector] public int connectPlayer = 0;
    [HideInInspector] public int maxPlayer = 0;

    public Text textRoomName;
    public Text textConnectInfo;

    [HideInInspector] public string ReadyState = ""; 


    void Start()
    {
        this.GetComponent<Button>().onClick.AddListener(() =>
        {
            Lobby_Mgr refPtInit = FindObjectOfType<Lobby_Mgr>();
            if(refPtInit != null )
               refPtInit.OnClickRoomItem(roomName);
        });
    }

  

public void DispRoomData(bool a_IsOpen)
    {
        Color color;
        if (a_IsOpen == true)
        {
            ColorUtility.TryParseHtmlString("#52CE90", out color); //초록색
            textRoomName.color = color;
            textConnectInfo.color = color;
        }
        else
        {
            ColorUtility.TryParseHtmlString("#4179A3", out color); //파란색
            textRoomName.color = color;
            textConnectInfo.color = color;
        }

        textRoomName.text = roomName;
        textConnectInfo.text = "(" + connectPlayer.ToString() + "/"
                                   + maxPlayer.ToString() + ")";
    }
}
