using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Damage : MonoBehaviour
{

    GameObject expEffect;
    PhotonView pv;

    public Canvas m_Canvas;

    public Text m_KillCnt;

    //플레이어 ID(고유번호) 저장하는 변수
    [HideInInspector] public int PlayerId = -1;


    int m_KillCount = 0;
    int m_Cur_LAttId = -1;

    ExitGames.Client.Photon.Hashtable KillProps =
                                      new ExitGames.Client.Photon.Hashtable();

    [HideInInspector] public float m_ReSetTime = 0.0f;

    int m_StCount = 0;
    Vector3 m_StPos = Vector3.zero;

    #region SingleTon
    public static Damage Inst = null;
    void Awake()
    {
        Inst = this;
    }
    #endregion

    void Start()
    {
        pv = GetComponent<PhotonView>();
        PlayerId = pv.Owner.ActorNumber;
        expEffect = Resources.Load<GameObject>("ExplosionMobile");
        InitCustomProperties(pv);
    }

    int m_UpdateCk = 2;
    void Update()
    {
        if (0 < m_UpdateCk)
        {
            m_UpdateCk--;
            if (m_UpdateCk <= 0)
            {
                ReadyState();
            }
        }

        if (0.0f < m_ReSetTime)
            m_ReSetTime -= Time.deltaTime;

        if (PhotonNetwork.CurrentRoom == null ||
            PhotonNetwork.LocalPlayer == null)
            return;

        if (m_KillCnt != null)
            m_KillCnt.text = m_KillCount.ToString();//킬 카운트 표시


    }

    public void ReadyState()
    {
        if (Ready_Mgr.m_GameState != GameState.Ready) return;


        StartCoroutine(WaitReady());
    }

    IEnumerator WaitReady()
    {
        while (Ready_Mgr.m_GameState != GameState.Ready)
        {
            yield return null;
        }

        //플레이어 리스폰
        string team = ReceiveTeam(PhotonNetwork.LocalPlayer);
        Vector3 spawnPos;

        if (team == "blue")
        {
            int index = PhotonNetwork.LocalPlayer.ActorNumber % Ready_Mgr.m_Team1Pos.Length;
            spawnPos = Ready_Mgr.m_Team1Pos[index];
        }
        else
        {
            int index = PhotonNetwork.LocalPlayer.ActorNumber % Ready_Mgr.m_Team2Pos.Length;
            spawnPos = Ready_Mgr.m_Team2Pos[index];
        }

        SetVisible(true);

        m_StPos = spawnPos;
        m_StCount = 5;
    }

    void OnTriggerEnter(Collider coll)
    {
        if (Game_Mgr.Inst.m_CurHP > 0 && coll.tag == "Bullet")
        {
            int a_AttId = -1;
            string a_AttTeam = "blue";
            Bullet_Ctrl a_RefBullet = coll.GetComponent<Bullet_Ctrl>();

            if (a_RefBullet != null)
            {
                a_AttId = a_RefBullet.AttackerId;
                a_AttTeam = a_RefBullet.AttackerTeam;
            }

            TakeDamage(a_AttId, a_AttTeam);
        }

    }

    public void TakeDamage(int AttackerId = -1, string a_AttTeam = "blue")
    {
        if (AttackerId == PlayerId) return; //자기 자신의 총알을 맞지않기위해 

        if (Game_Mgr.Inst.m_CurHP <= 0) return;

        if (pv.IsMine == false) return;

        if (0 < m_ReSetTime) return;

        string a_DmgTeam = "blue";

        if (pv.Owner.CustomProperties.ContainsKey("MyTeam") == true)
            a_DmgTeam = (string)pv.Owner.CustomProperties["MyTeam"];


        if (a_AttTeam == a_DmgTeam) return; //팀킬 방지

        m_Cur_LAttId = AttackerId;
        Game_Mgr.Inst.m_CurHP -= 20;
        if (Game_Mgr.Inst.m_CurHP < 0)
            Game_Mgr.Inst.m_CurHP = 0;

        Game_Mgr.Inst.m_HPBar.fillAmount = Game_Mgr.Inst.m_CurHP / Game_Mgr.Inst.m_MaxHP;

        if (Game_Mgr.Inst.m_CurHP <= 0)
        {

        }


    }

    IEnumerator Explosion()
    {
        GameObject effect = Instantiate(expEffect, transform.position, Quaternion.identity);

        Destroy(effect, 2.0f);

        SetVisible(false);

        yield return null;
    }

    void SetVisible(bool IsVisible)
    {

    }

    void AvartaUpdate()
    {
        if (0 < Game_Mgr.Inst.m_CurHP)
        {
            Game_Mgr.Inst.m_CurHP = 0;

            Game_Mgr.Inst.m_HPBar.fillAmount = Game_Mgr.Inst.m_CurHP / Game_Mgr.Inst.m_MaxHP;

            if (Game_Mgr.Inst.m_CurHP <= 0)
            {
                Game_Mgr.Inst.m_CurHP = 0;

                if (0 <= m_Cur_LAttId)
                {
                    SaveKillCount(m_Cur_LAttId);
                }
                StartCoroutine(Explosion());
            }


        }

        else
        {
            Game_Mgr.Inst.m_CurHP = 0;

            Game_Mgr.Inst.m_HPBar.fillAmount = 1;

            m_Canvas.enabled = true;

            Game_Mgr.Inst.m_CurHP = Game_Mgr.Inst.m_MaxHP;

            SetVisible(true);

        }


    }

    void SaveKillCount(int AttackerId)
    {
        GameObject[] a_Players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject a_Player in a_Players)
        {
            var a_Dmg = a_Player.GetComponent<Damage>();

            if (a_Dmg != null && a_Dmg.PlayerId == AttackerId)
            {
                if (a_Dmg.InKillCount() == true)
                {
                    return;
                }
            }
        }
    }


    public bool InKillCount()
    {
        if (pv != null && pv.IsMine == true)
        {
            m_KillCount++;


            SendKillCount(m_KillCount);
            return true;
        }

        return false;
    }


    void InitCustomProperties(PhotonView pv)
    {
        if (pv != null && pv.IsMine == true)
        {
            KillProps.Clear();
            KillProps.Add("KillCount", 0);
            pv.Owner.SetCustomProperties(KillProps);

        }

    }

    void SendKillCount(int a_KillCount = 0)
    {
        if (pv == null && pv.IsMine == false) return;

        if (KillProps == null)
        {
            KillProps = new ExitGames.Client.Photon.Hashtable();
            KillProps.Clear();
        }

        if (KillProps.ContainsKey("KillCount") == true)
            KillProps["KillCount"] = a_KillCount;
        else
            KillProps.Add("KillCount", a_KillCount);

        pv.Owner.SetCustomProperties(KillProps);

    }

    void ReceiveKillCount()
    {
        if (pv == null && pv.IsMine == true && pv.Owner == null) return;//원격 플레이어만 받음

        if (pv.Owner.CustomProperties.ContainsKey("KillCount") == true)
            m_KillCount = (int)pv.Owner.CustomProperties["KillCount"];

    }

    string ReceiveTeam(Player a_Player)
    {
        string a_TeamKind = "blue";

        if (a_Player == null) return a_TeamKind;

        if (a_Player.CustomProperties.ContainsKey("MyTeam") == true)
            a_TeamKind = (string)a_Player.CustomProperties["MyTeam"];

        return a_TeamKind;

    }
    int ReceiveSitPosInx(Player a_Player)
    {
        int a_SitIdx = -1;

        if (a_Player == null)
            return a_SitIdx;

        if (a_Player.CustomProperties.ContainsKey("SitPosInx") == true)
            a_SitIdx = (int)a_Player.CustomProperties["SitPosInx"];

        return a_SitIdx;
    }

}
