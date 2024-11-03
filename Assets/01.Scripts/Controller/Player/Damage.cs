using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damage : MonoBehaviour
{
    //폭발 효과 프리팹을 연결할 변수
    private GameObject expEffect = null;
    PhotonView pv = null;


    //플레이어 ID(고유번호) 저장하는 변수
    [HideInInspector] public int PlayerId = -1;

    int m_Cur_LAttId = -1;

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
    }

    int m_UpdateCk = 2;
    private void Update()
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

        if (pv.Owner.CustomProperties.ContainsKey("MyTeam")==true)
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
