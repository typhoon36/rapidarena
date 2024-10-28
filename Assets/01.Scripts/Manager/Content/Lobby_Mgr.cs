using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Lobby_Mgr : MonoBehaviourPunCallbacks
{
    #region Top_Panel
    [Header("Top_Panel")]
    public Button m_PlayBtn;
    public Button m_LoadOutBtn;
    public Button m_ShopBtn;

    public Button m_ExitBtn;
    public Button m_HomeBtn;
    public GameObject m_RoomPanel;

    [Header("Setting")]
    public Button m_SettingBtn;
    public Transform Canvas_Parent;
    public GameObject m_ConfigObj;
    #endregion

    #region PhotonInit
    [Header("PhotonInit")]
    public InputField userId;
    public InputField roomName;
    public Button CreateRoomBtn;
    public GameObject scrollContents;
    public GameObject roomItem;
    RoomItem[] m_RoomItemList;
    #endregion


    void Awake()
    {
        if (!PhotonNetwork.IsConnected)
            PhotonNetwork.ConnectUsingSettings();

        userId.text = GetUserId();

        roomName.text = "Room_" + Random.Range(0, 999).ToString("000");
    }

    void Start()
    {
        if (CreateRoomBtn != null)
            CreateRoomBtn.onClick.AddListener(OnClickCreateRoom);

        if (m_PlayBtn != null)
            m_PlayBtn.onClick.AddListener(() =>
            {
                m_RoomPanel.SetActive(true);
            });

        if (m_ShopBtn != null)
            m_ShopBtn.onClick.AddListener(() =>
            {
                SceneManager.LoadScene("ShopScene");
            });
        if (m_LoadOutBtn != null)
            m_LoadOutBtn.onClick.AddListener(() =>
            {
                SceneManager.LoadScene("Inven_Scene");
            });

        if (m_ExitBtn != null)
            m_ExitBtn.onClick.AddListener(() =>
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
                        Application.Quit();
#endif
            });

        if (m_HomeBtn != null)
        {
            m_HomeBtn.onClick.AddListener(() =>
            {
            
                SceneManager.LoadScene("Pt_LobbyScene");
            });
        }

        if (m_SettingBtn != null)
        {
            m_SettingBtn.onClick.AddListener(() =>
            {
                Instantiate(m_ConfigObj,Canvas_Parent);
            });
        }
    }

    #region Photon methods
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby(); //가상 로비 접속
    }

    public override void OnJoinedLobby()
    {
        userId.text = GetUserId();
    }

    public void ClickJoinRandomRoom()
    {
        PhotonNetwork.LocalPlayer.NickName = userId.text;

        PlayerPrefs.SetString("USER_ID", userId.text);

        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinedRoom()
    {
        StartCoroutine(this.LoadBattleField());
    }

    //룸 씬으로 이동하는 코루틴 함수
    IEnumerator LoadBattleField()
    {
        PhotonNetwork.IsMessageQueueRunning = false;

        Time.timeScale = 1.0f;

        AsyncOperation ao = SceneManager.LoadSceneAsync("GameScene");

        yield return ao;
    }

    void OnClickCreateRoom()
    {
        string _roomName = roomName.text;
        if (string.IsNullOrEmpty(roomName.text))
        {
            _roomName = "Room_" + Random.Range(0, 999).ToString("000");
        }

        PhotonNetwork.LocalPlayer.NickName = userId.text;
        PlayerPrefs.SetString("USER_ID", userId.text);

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.IsOpen = true;
        roomOptions.IsVisible = true;
        roomOptions.MaxPlayers = 8;

        PhotonNetwork.CreateRoom(_roomName, roomOptions, TypedLobby.Default);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log(returnCode.ToString());
        Debug.Log(message);
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        m_RoomItemList = scrollContents.transform.GetComponentsInChildren<RoomItem>(true);

        int roomCount = roomList.Count;
        int a_ArrIdx = 0;
        for (int i = 0; i < roomCount; i++)
        {
            a_ArrIdx = MyFindIndex(m_RoomItemList, roomList[i]);

            if (roomList[i].RemovedFromList == false)
            {
                if (a_ArrIdx < 0)
                {
                    GameObject room = Instantiate(roomItem) as GameObject;

                    room.transform.SetParent(scrollContents.transform, false);

                    RoomItem roomData = room.GetComponent<RoomItem>();
                    roomData.roomName = roomList[i].Name;
                    roomData.connectPlayer = roomList[i].PlayerCount;
                    roomData.maxPlayer = roomList[i].MaxPlayers;

                    roomData.DispRoomData(roomList[i].IsOpen);
                }
                else
                {
                    m_RoomItemList[a_ArrIdx].roomName = roomList[i].Name;
                    m_RoomItemList[a_ArrIdx].connectPlayer = roomList[i].PlayerCount;
                    m_RoomItemList[a_ArrIdx].maxPlayer = roomList[i].MaxPlayers;

                    m_RoomItemList[a_ArrIdx].DispRoomData(roomList[i].IsOpen);
                }
            }
            else
            {
                if (0 <= a_ArrIdx)
                {
                    MyDestroy(m_RoomItemList, roomList[i]);
                }
            }
        }
    }

    int MyFindIndex(RoomItem[] a_RmItemList, RoomInfo a_RoomInfo)
    {
        if (a_RmItemList == null)
            return -1;

        if (a_RmItemList.Length <= 0)
            return -1;

        for (int i = 0; i < a_RmItemList.Length; i++)
        {
            if (a_RmItemList[i].roomName == a_RoomInfo.Name)
            {
                return i;
            }
        }

        return -1;
    }

    void MyDestroy(RoomItem[] a_RmItemList, RoomInfo a_RoomInfo)
    {
        if (a_RmItemList == null)
            return;

        if (a_RmItemList.Length <= 0)
            return;

        for (int i = 0; i < a_RmItemList.Length; i++)
        {
            if (a_RmItemList[i].roomName == a_RoomInfo.Name)
            {
                Destroy(a_RmItemList[i].gameObject);
            }
        }
    }

    public void OnClickRoomItem(string roomName)
    {
        PhotonNetwork.LocalPlayer.NickName = userId.text;
        PlayerPrefs.SetString("USER_ID", userId.text);
        PhotonNetwork.JoinRoom(roomName);
    }

    string GetUserId()
    {
        string userId = PlayerPrefs.GetString("USER_ID");

        if (string.IsNullOrEmpty(userId))
        {
            userId = "USER_" + Random.Range(0, 999).ToString("000");
        }

        return userId;
    }
    #endregion

    #region UI Methods

    #endregion
}
