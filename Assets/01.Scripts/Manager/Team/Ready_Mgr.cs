using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using System.Collections.Generic;


public class Ready_Mgr : MonoBehaviourPunCallbacks
{
    public static bool IsFocus = true;

    #region Connect Info
    public GameObject m_ChatPanel;
    public Text txtConnect;
    public Button ExitRoomBtn;
    public Text txtLogMsg;
    #endregion

    PhotonView pv;

    #region Chatting
    public InputField InputFdChat;
    public static bool IsEnter = false;
    #endregion

    #region Team 대전 변수
    [HideInInspector] public GameState m_OldState = GameState.Ready;
    public static GameState m_GameState = GameState.Ready;

    ExitGames.Client.Photon.Hashtable m_StateProps =
                            new ExitGames.Client.Photon.Hashtable();

    ExitGames.Client.Photon.Hashtable m_SelTeamProps =
                               new ExitGames.Client.Photon.Hashtable();

    ExitGames.Client.Photon.Hashtable m_PlayerReady =
                                new ExitGames.Client.Photon.Hashtable();

    ExitGames.Client.Photon.Hashtable SitPosInxProps =
                                new ExitGames.Client.Photon.Hashtable();

    [HideInInspector] public static Vector3[] m_Team1Pos = new Vector3[4];
    [HideInInspector] public static Vector3[] m_Team2Pos = new Vector3[4];
    #endregion

    #region TeamSelect
    [Header("Team1")]
    public GameObject Team1Panel;
    public Button m_Team1ToTeam2;
    public Button m_Team1Ready;
    public GameObject scrollTeam1;

    [Header("Team2")]
    public GameObject Team2Panel;
    public Button m_Team2ToTeam1;
    public Button m_Team2Ready;
    public GameObject scrollTeam2;

    [Header("--- User Node ---")]
    public GameObject m_UserNodeItem;

    [Header("--- StartTimer UI ---")]
    public Text m_WaitTmText;       //게임 시작 후 카운트 3, 2, 1, 0
    [HideInInspector] public float m_GoWaitGame = 4.0f;    //게임 시작 후 카운트 Text UI
    #endregion

    void Awake()
    {
        m_Team1Pos[0] = new Vector3(31.3f, 4f, -75f);
        m_Team1Pos[1] = new Vector3(31.3f, 4f, -77f);
        m_Team1Pos[2] = new Vector3(31.3f, 4f, -79f);
        m_Team1Pos[3] = new Vector3(31.3f, 4, -82f);

        m_Team2Pos[0] = new Vector3(102.3f, 4f, -77.24f);
        m_Team2Pos[1] = new Vector3(102.3f, 4f, -79.14f);
        m_Team2Pos[2] = new Vector3(102.3f, 4f, -81.10f);
        m_Team2Pos[3] = new Vector3(102.3f, 4f, -83.42f);

        m_GameState = GameState.Ready;



        IsEnter = false;

        pv = GetComponent<PhotonView>();

        CreatePlayer();

        PhotonNetwork.IsMessageQueueRunning = true;


        GetConnectPlayerCount();


        #region Custom Properties Init
        InitGStateProps();
        InitSelTeamProps();
        InitReadyProps();
        #endregion

    }

    void Start()
    {
        #region TeamSetting
        if (m_Team1ToTeam2 != null)
            m_Team1ToTeam2.onClick.AddListener(() =>
            {
                SendSelTeam("red"); 
            });

        if (m_Team1Ready != null)
            m_Team1Ready.onClick.AddListener(() =>
            {
                SendReady(1);
            });

        if (m_Team2ToTeam1 != null)
            m_Team2ToTeam1.onClick.AddListener(() =>
            {
                SendSelTeam("blue");  
            });

        if (m_Team2Ready != null)
            m_Team2Ready.onClick.AddListener(() =>
            {
                SendReady(1);
            });

        #endregion 

        if (ExitRoomBtn != null)
            ExitRoomBtn.onClick.AddListener(OnClickExitRoom);

        //로그 메시지에 출력할 문자열 생성
        string msg = "\n<color=#00ff00>["
                      + PhotonNetwork.LocalPlayer.NickName
                      + "] Connected</color>";

        //RPC 함수 호출
        pv.RPC("LogMsg", RpcTarget.AllBuffered, msg, false);

    }
    void Update()
    {
        // 게임 Update()를 돌려도 되는 상태인지 확인한다.
        if (IsGamePossible() == false)
        {
            return;
        }

        if (m_GameState == GameState.Ready)
        {
            if (IsDifferentList() == true)
            {
                RefreshPhotonTeam();
            }
        }

        #region Chatting
        if(Input.GetKeyDown(KeyCode.Return) && m_GameState == GameState.Ready)
        {
            IsEnter = !IsEnter;

            if (InputFdChat.gameObject.activeSelf)
            {
                InputFdChat.gameObject.SetActive(false);
                BroadcastingChat();
            }
            else
            {
                InputFdChat.gameObject.SetActive(true);
            }
        }
        #endregion

        AllReadyObserver();

        if (m_GameState == GameState.Play)
        {
            Team1Panel.SetActive(false);
            Team2Panel.SetActive(false);
            m_WaitTmText.gameObject.SetActive(false);
        }
    }

    void BroadcastingChat()
    {
        string msg = "\n<color=#ffffff>[" +
                       PhotonNetwork.LocalPlayer.NickName + "] " +
                       InputFdChat.text + "</color>";

        pv.RPC("LogMsg", RpcTarget.AllBuffered, msg, true);

        InputFdChat.text = "";

    }

    void CreatePlayer()
    {
        string team = ReceiveSelTeam(PhotonNetwork.LocalPlayer);
        Vector3 spawnPos;

        if (team == "blue")
        {
            int index = PhotonNetwork.LocalPlayer.ActorNumber % m_Team1Pos.Length;
            spawnPos = m_Team1Pos[index];
        }
        else
        {
            int index = PhotonNetwork.LocalPlayer.ActorNumber % m_Team2Pos.Length;
            spawnPos = m_Team2Pos[index];
        }

        PhotonNetwork.Instantiate("Player", spawnPos, Quaternion.identity, 0);
    }
    void OnApplicationFocus(bool a_Focus)
    {
        IsFocus = a_Focus;
    }
    void GetConnectPlayerCount()
    {
        Room currRoom = PhotonNetwork.CurrentRoom;

        if (currRoom != null)
        {
            txtConnect.text = currRoom.PlayerCount.ToString() + "/" + currRoom.MaxPlayers.ToString();
        }
    }

    //접속시
    public override void OnPlayerEnteredRoom(Player a_Player)
    {
        GetConnectPlayerCount();
    }

    //나갔을시
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        GetConnectPlayerCount();
    }

    //현재 방 나가기 함수(버튼에 연결되어 있음)
    public void OnClickExitRoom()
    {
        string msg = "\n<color=#ff0000>["
                      + PhotonNetwork.LocalPlayer.NickName
                      + "] Disconnected</color>";

        //RPC 함수 호출
        pv.RPC("LogMsg", RpcTarget.AllBuffered, msg, false);


        if (PhotonNetwork.PlayerList != null && PhotonNetwork.PlayerList.Length <= 1)
        {
            if (PhotonNetwork.CurrentRoom != null)
                PhotonNetwork.CurrentRoom.CustomProperties.Clear();
        }
    
        if (PhotonNetwork.LocalPlayer != null)
            PhotonNetwork.LocalPlayer.CustomProperties.Clear();

        //실질적으로 방을 나가고 모든 네트워크 객체 삭제
        PhotonNetwork.LeaveRoom();
    }

    //접속 종료시 콜백.
    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("Pt_LobbyScene");
    }

    bool IsGamePossible()
    {
        if (PhotonNetwork.CurrentRoom == null ||
           PhotonNetwork.LocalPlayer == null)
            return false;

        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("GameState") == false)
            return false;


        m_GameState = ReceiveGState();


        return true;
    }

    List<string> m_MsgList = new List<string>();

    [PunRPC]
    void LogMsg(string msg, bool isChatMsg, PhotonMessageInfo info)
    {
        if (info.Sender.IsLocal && isChatMsg == true)
            msg = msg.Replace("#ffffff", "#ffff00");
        

        m_MsgList.Add(msg);
        if (20 < m_MsgList.Count)
            m_MsgList.RemoveAt(0);

        txtLogMsg.text = "";
        for (int i = 0; i < m_MsgList.Count; i++)
        {
            txtLogMsg.text += m_MsgList[i];
        }
    }



    #region ---------- 게임 상태 동기화 처리

    void InitGStateProps()
    {
        if (PhotonNetwork.CurrentRoom == null)
            return;

        m_StateProps.Clear();
        m_StateProps.Add("GameState", (int)GameState.Ready);
        PhotonNetwork.CurrentRoom.SetCustomProperties(m_StateProps);
    }

    public void SendGState(GameState a_GState)
    {
        if (m_StateProps == null)
        {
            m_StateProps = new ExitGames.Client.Photon.Hashtable();
            m_StateProps.Clear();
        }

        if (m_StateProps.ContainsKey("GameState") == true)
            m_StateProps["GameState"] = (int)a_GState;
        else
            m_StateProps.Add("GameState", (int)a_GState);

        PhotonNetwork.CurrentRoom.SetCustomProperties(m_StateProps);
    }

    GameState ReceiveGState()
    {
        GameState a_RmVal = GameState.Ready;

        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("GameState") == true)
            a_RmVal = (GameState)PhotonNetwork.CurrentRoom.CustomProperties["GameState"];

        return a_RmVal;
    }

    #endregion

    #region --------- 팀선택 동기화 처리
    void InitSelTeamProps()
    {
        m_SelTeamProps.Clear();
        m_SelTeamProps.Add("MyTeam", "blue");
        PhotonNetwork.LocalPlayer.SetCustomProperties(m_SelTeamProps);
    }

    void SendSelTeam(string a_Team)
    {
        if (string.IsNullOrEmpty(a_Team) == true)
            return;

        if (m_SelTeamProps == null)
        {
            m_SelTeamProps = new ExitGames.Client.Photon.Hashtable();
            m_SelTeamProps.Clear();
        }

        if (m_SelTeamProps.ContainsKey("MyTeam") == true)
            m_SelTeamProps["MyTeam"] = a_Team;
        else
            m_SelTeamProps.Add("MyTeam", a_Team);

        PhotonNetwork.LocalPlayer.SetCustomProperties(m_SelTeamProps);

    }

    public string ReceiveSelTeam(Player a_Player)
    {
        string a_TeamKind = "blue";

        if (a_Player == null)
            return a_TeamKind;

        if (a_Player.CustomProperties.ContainsKey("MyTeam") == true)
            a_TeamKind = (string)a_Player.CustomProperties["MyTeam"];

        return a_TeamKind;
    }

    bool IsDifferentList()
    {
        GameObject[] a_UserNodeItems = GameObject.FindGameObjectsWithTag("UserNode_Item");

        if (a_UserNodeItems == null)
            return true;

        if (PhotonNetwork.PlayerList.Length != a_UserNodeItems.Length)
            return true;

        foreach (Player a_RefPlayer in PhotonNetwork.PlayerList)
        {
            bool a_FindNode = false;
            UserNodeItem a_UserData = null;
            foreach (GameObject a_Node in a_UserNodeItems)
            {
                a_UserData = a_Node.GetComponent<UserNodeItem>();
                if (a_UserData == null)
                    continue;

                if (a_UserData.m_UniqID == a_RefPlayer.ActorNumber)
                {
                    if (a_UserData.m_TeamKind != ReceiveSelTeam(a_RefPlayer))
                        return true;

                    if (a_UserData.m_IamReady != ReceiveReady(a_RefPlayer))
                        return true;

                    a_FindNode = true;
                    break;
                }
            }

            if (a_FindNode == false)
                return true;

        }

        return false;

    }

    void RefreshPhotonTeam() //각 팀의 리스트뷰 UI 를 갱신해 주는 함수
    {

        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("UserNode_Item"))
        {
            Destroy(obj);
        }

        string a_TeamKind = "blue";
        GameObject a_UserNode = null;
        foreach (Player a_RefPlayer in PhotonNetwork.PlayerList)
        {
            a_TeamKind = ReceiveSelTeam(a_RefPlayer);
            a_UserNode = Instantiate(m_UserNodeItem);

            // 팀이 뭐냐?에 따라서 스크롤 뷰를 분기 해 준다.
            if (a_TeamKind == "blue")
                a_UserNode.transform.SetParent(scrollTeam1.transform, false);
            else if (a_TeamKind == "red")
                a_UserNode.transform.SetParent(scrollTeam2.transform, false);

            // 생성한 UserNodeItem에 표시하기 위한 텍스트 정보 전달
            UserNodeItem a_UsData = a_UserNode.GetComponent<UserNodeItem>();
            if (a_UsData != null)
            {
                a_UsData.m_UniqID = a_RefPlayer.ActorNumber;
                a_UsData.m_TeamKind = a_TeamKind;
                a_UsData.m_IamReady = ReceiveReady(a_RefPlayer);
                bool isMine = (a_UsData.m_UniqID == PhotonNetwork.LocalPlayer.ActorNumber);
                a_UsData.DispPlayerData(a_RefPlayer.NickName, isMine);
            }

        }

        //--- 나의 Ready 상태에 따라서 UI 변경해 주기
        if (ReceiveReady(PhotonNetwork.LocalPlayer) == true)
        {  //내가 Ready 상태라면...
            m_Team1Ready.gameObject.SetActive(false);
            m_Team2Ready.gameObject.SetActive(false);

            m_Team1ToTeam2.gameObject.SetActive(false);
            m_Team2ToTeam1.gameObject.SetActive(false);
        }
        else  //내가 아직 Ready 상태가 아니라면...
        {
            a_TeamKind = ReceiveSelTeam(PhotonNetwork.LocalPlayer);
            if (a_TeamKind == "blue")
            {
                m_Team1Ready.gameObject.SetActive(true);
                m_Team2Ready.gameObject.SetActive(false);
                m_Team1ToTeam2.gameObject.SetActive(true);
                m_Team2ToTeam1.gameObject.SetActive(false);
            }
            else if (a_TeamKind == "red")
            {
                m_Team1Ready.gameObject.SetActive(false);
                m_Team2Ready.gameObject.SetActive(true);
                m_Team1ToTeam2.gameObject.SetActive(false);
                m_Team2ToTeam1.gameObject.SetActive(true);
            }
        }
    }
    #endregion

    #region --------- Ready 상태 동기화 처리

    void InitReadyProps()
    { //속도를 위해 버퍼를 미리 만들어 놓는다는 의미
        m_PlayerReady.Clear();
        m_PlayerReady.Add("IamReady", 0);   //기본적으로 아직 준비전 상태로 시작한다.
        PhotonNetwork.LocalPlayer.SetCustomProperties(m_PlayerReady);
    }

    void SendReady(int a_Ready = 1)
    {
        if (m_PlayerReady == null)
        {
            m_PlayerReady = new ExitGames.Client.Photon.Hashtable();
            m_PlayerReady.Clear();
        }

        if (m_PlayerReady.ContainsKey("IamReady") == true)
            m_PlayerReady["IamReady"] = a_Ready;
        else
            m_PlayerReady.Add("IamReady", a_Ready);

        PhotonNetwork.LocalPlayer.SetCustomProperties(m_PlayerReady);
    }

    bool ReceiveReady(Player a_Player)
    {
        if (a_Player == null)
            return false;

        if (a_Player.CustomProperties.ContainsKey("IamReady") == false)
            return false;

        if ((int)a_Player.CustomProperties["IamReady"] == 1)
            return true;

        return false;
    }

    #endregion

    #region ------------ Observer Method 모음

    void AllReadyObserver()
    {
        if (m_GameState != GameState.Ready)
            return;

        int a_OldGoWait = (int)m_GoWaitGame;

        bool a_AllReady = true;
        foreach (Player a_RefPlayer in PhotonNetwork.PlayerList)
        {
            if (ReceiveReady(a_RefPlayer) == false)
            {
                a_AllReady = false;
                break;
            }
        }

        if (a_AllReady == true)
        {

            if (Game_Mgr.Inst.m_RoundCnt == 0 && PhotonNetwork.CurrentRoom.IsOpen == true)
                PhotonNetwork.CurrentRoom.IsOpen = false;

            if (0.0f < m_GoWaitGame)
            {
                m_GoWaitGame -= Time.deltaTime;
                if (m_WaitTmText != null)
                {
                    m_WaitTmText.gameObject.SetActive(true);
                    m_WaitTmText.text = ((int)m_GoWaitGame).ToString();
                }


                if (PhotonNetwork.IsMasterClient == true)
                {
                    if (0.0f < m_GoWaitGame && a_OldGoWait != (int)m_GoWaitGame)
                    {

                        SitPosInxMasterCtrl();
                    }
                }

                if (m_GoWaitGame <= 0.0f)
                {
                    Game_Mgr.Inst.m_RoundCnt++;

                    Team1Panel.SetActive(false);
                    Team2Panel.SetActive(false);
                    m_WaitTmText.gameObject.SetActive(false);
                    InputFdChat.gameObject.SetActive(false);
                    m_ChatPanel.SetActive(false);

                    m_GoWaitGame = 0.0f;

                    Game_Mgr.Inst.m_GameObj.SetActive(true);

                }

            }



            if (PhotonNetwork.IsMasterClient == true)
                if (m_GoWaitGame <= 0.0f)
                {
                    SendGState(GameState.Play);
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                }

        }
    }

    void SitPosInxMasterCtrl()
    {
        int a_Tm1Count = 0;
        int a_Tm2Count = 0;
        string a_TeamKind = "blue";
        foreach (Player _player in PhotonNetwork.PlayerList)
        {
            if (_player.CustomProperties.ContainsKey("MyTeam") == true)
                a_TeamKind = (string)_player.CustomProperties["MyTeam"];

            if (a_TeamKind == "blue")
            {
                SitPosInxProps.Clear();
                SitPosInxProps.Add("SitPosInx", a_Tm1Count);
                _player.SetCustomProperties(SitPosInxProps);
                a_Tm1Count++;
            }
            else if (a_TeamKind == "red")
            {
                SitPosInxProps.Clear();
                SitPosInxProps.Add("SitPosInx", a_Tm2Count);
                _player.SetCustomProperties(SitPosInxProps);
                a_Tm2Count++;
            }
        }

      
    }
    #endregion

}
