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
    }
}
