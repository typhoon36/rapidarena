using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 팀 매니저
public class Team_Mgr : MonoBehaviourPunCallbacks
{
    [HideInInspector] public double m_CheckWinTime = 2.0f;
    int IsRoomBuf_Team1Win = 0;
    [HideInInspector] public int m_Team1Win = 0;
    int IsRoomBuf_Team2Win = 0;
    [HideInInspector] public int m_Team2Win = 0;

    ExitGames.Client.Photon.Hashtable m_Team1WinProps = new ExitGames.Client.Photon.Hashtable();
    ExitGames.Client.Photon.Hashtable m_Team2WinProps = new ExitGames.Client.Photon.Hashtable();

    #region SingleTon
    public static Team_Mgr Inst = null;

    void Awake()
    {
        Inst = this;
    }
    #endregion

    void Start()
    {
        InitTeam1WinProps();
        InitTeam2WinProps();
    }

    //한쪽팀이 전멸했는지 체크하고 승리 / 패배를 감시하고 처리해 주는 함수
    public void WinLossObserver(Ready_Mgr a_ReadyMgr)
    {
        if (a_ReadyMgr == null)
            return;

        //---- 승리 / 패배 체크
        if (Ready_Mgr.m_GameState == GameState.Play)
        {
            m_CheckWinTime -= Time.deltaTime;
            if (m_CheckWinTime <= 0.0f)
            {
                CheckAliveTeam(a_ReadyMgr);
            }
        }

        if (Game_Mgr.Inst.m_WinLoseTxt != null)
            Game_Mgr.Inst.m_WinLoseTxt.text = "<color=Blue>" + "Team1 : " +
                                              m_Team1Win.ToString() + "</color> : " +
                                              "<color=red>" + "Team2 : " +
                                              m_Team2Win.ToString() + "</color>";

        if (5 <= (m_Team1Win + m_Team2Win))
        {
            if (PhotonNetwork.IsMasterClient == true)
                a_ReadyMgr.SendGState(GameState.End);

            if (Game_Mgr.Inst.m_GameEndText != null)
            {
                Game_Mgr.Inst.m_GameEndText.gameObject.SetActive(true);
                if (m_Team1Win < m_Team2Win)
                {
                    Game_Mgr.Inst.m_GameEndText.text = "<color=Black>" + "레드팀 승" + "</color>";
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                }
                else
                {
                    Game_Mgr.Inst.m_GameEndText.text = "<color=Blue>" + "블루팀 승" + "</color>";
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                }
            }

            if (a_ReadyMgr.m_WaitTmText != null)
                a_ReadyMgr.m_WaitTmText.gameObject.SetActive(false);

            return;
        }

        if (a_ReadyMgr.m_OldState != GameState.Ready && Ready_Mgr.m_GameState == GameState.Ready)
        {
            GameObject[] tanks = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject tank in tanks)
            {
                Damage a_Damage = tank.GetComponent<Damage>();
                if (a_Damage != null)
                    a_Damage.ReadyState();
            }
        }
        a_ReadyMgr.m_OldState = Ready_Mgr.m_GameState;
    }

    //한쪽팀이 전멸했는지 체크하는 함수
    void CheckAliveTeam(Ready_Mgr a_ReadyMgr)
    {
        int a_Tm1Count = 0;
        int a_Tm2Count = 0;
        int rowTm1 = 0;
        int rowTm2 = 0;
        string a_PlrTeam = "blue";  //Player Team

        GameObject[] Players = GameObject.FindGameObjectsWithTag("Player");

        Player[] players = PhotonNetwork.PlayerList;
        foreach (Player a_player in players)
        {
            if (a_player.CustomProperties.ContainsKey("MyTeam") == true)
                a_PlrTeam = (string)a_player.CustomProperties["MyTeam"];

            Damage a_Damage = null;
            foreach (GameObject pl in Players)
            {
                Damage a_Dmg = pl.GetComponent<Damage>();
                if (a_Dmg == null)
                    continue;

                if (a_Dmg.PlayerId == a_player.ActorNumber)
                {
                    a_Damage = a_Dmg;
                    break;
                }
            }

            if (a_PlrTeam == "blue")
            {
                if (a_Damage != null && 0 < Game_Mgr.Inst.m_CurHP)
                    rowTm1 = 1;
                a_Tm1Count++;
            }
            else if (a_PlrTeam == "red")
            {
                if (a_Damage != null && 0 < Game_Mgr.Inst.m_CurHP)
                    rowTm2 = 1;
                a_Tm2Count++;
            }
        }

        a_ReadyMgr.m_GoWaitGame = 4.0f;

        if (0 < rowTm1 && 0 < rowTm2)
            return;

        if (5 <= (m_Team1Win + m_Team2Win))
            return;

        if (PhotonNetwork.IsMasterClient == false)
            return;

        a_ReadyMgr.SendGState(GameState.Ready);

        if (rowTm1 == 0)
        {
            if (-99999.0f < m_CheckWinTime)
            {
                m_Team2Win++;

                if (Ready_Mgr.m_GameState != GameState.End && a_Tm1Count <= 0)
                    m_Team2Win = 5 - m_Team1Win;

                IsRoomBuf_Team2Win = m_Team2Win;
                m_CheckWinTime = -150000.0f;
            }
            SendTeam2Win(IsRoomBuf_Team2Win);
        }
        else if (rowTm2 == 0)
        {
            if (-99999.0f < m_CheckWinTime)
            {
                m_Team1Win++;

                if (Ready_Mgr.m_GameState != GameState.End && a_Tm2Count <= 0)
                    m_Team1Win = 5 - m_Team2Win;

                IsRoomBuf_Team1Win = m_Team1Win;
                m_CheckWinTime = -150000.0f;
            }
            SendTeam1Win(IsRoomBuf_Team1Win);
        }
    }

    #region ------ Team1 Win Count
    void InitTeam1WinProps()
    {
        if (PhotonNetwork.CurrentRoom == null)
            return;

        m_Team1WinProps.Clear();
        m_Team1WinProps.Add("Team1Win", 0);
        PhotonNetwork.CurrentRoom.SetCustomProperties(m_Team1WinProps);
    }

    void SendTeam1Win(int a_WinCount)
    {
        if (m_Team1WinProps == null)
        {
            m_Team1WinProps = new ExitGames.Client.Photon.Hashtable();
            m_Team1WinProps.Clear();
        }

        if (m_Team1WinProps.ContainsKey("Team1Win") == true)
            m_Team1WinProps["Team1Win"] = a_WinCount;
        else
            m_Team1WinProps.Add("Team1Win", a_WinCount);

        PhotonNetwork.CurrentRoom.SetCustomProperties(m_Team1WinProps);
    }
    #endregion

    #region --------------- Team2 Win Count
    void InitTeam2WinProps()
    {
        if (PhotonNetwork.CurrentRoom == null)
            return;

        m_Team2WinProps.Clear();
        m_Team2WinProps.Add("Team2Win", 0);
        PhotonNetwork.CurrentRoom.SetCustomProperties(m_Team2WinProps);
    }

    void SendTeam2Win(int a_WinCount)
    {
        if (m_Team2WinProps == null)
        {
            m_Team2WinProps = new ExitGames.Client.Photon.Hashtable();
            m_Team2WinProps.Clear();
        }

        if (m_Team2WinProps.ContainsKey("Team2Win") == true)
            m_Team2WinProps["Team2Win"] = a_WinCount;
        else
            m_Team2WinProps.Add("Team2Win", a_WinCount);

        PhotonNetwork.CurrentRoom.SetCustomProperties(m_Team2WinProps);
    }
    #endregion
}
