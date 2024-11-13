using Photon.Pun;
using Photon.Pun.Demo.Asteroids;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Damage : MonoBehaviourPunCallbacks, IPunObservable
{
    GameObject _Effect;

    int InitHp = 440;
    [HideInInspector] public int m_CurHP = 440;
    int NetHP = 440;

    #region 킬카운트 
    [Header("KillCount")]
    public Canvas m_Canvas;
    [HideInInspector] public int PlayerId = -1;
    #endregion

    Image m_HPBar;
    //Private인 이유는 Game_Mgr에서 가져와 유저들이 사용해주기 위함.

    #region Photon
    PhotonView pv;
    int m_KillCount = 0;
    int m_Cur_LAttId = -1;
    ExitGames.Client.Photon.Hashtable KillProps =
                                        new ExitGames.Client.Photon.Hashtable();

    [HideInInspector] public float m_ReSetTime = 0.0f;

    [HideInInspector] public int m_StCount = 0;
    Vector3 m_StPos = Vector3.zero;
    #endregion

    void Start()
    {
        pv = GetComponent<PhotonView>();
        PlayerId = pv.Owner.ActorNumber;//Owner: 해당 오브젝트를 소유한 플레이어

        if (pv.IsMine == true)
        {
            InitHp = m_CurHP;//초기화
            m_HPBar = Game_Mgr.Inst.m_HPBar;
            m_HPBar.fillAmount = 1.0f;
            m_HPBar.color = Color.green;
        }

        _Effect = Resources.Load<GameObject>("ExplosionMobile");
    }

    int m_UpdateCk = 2;
    void Update()
    {
        if (0 < m_UpdateCk)
        {
            m_UpdateCk--;
            if (m_UpdateCk <= 0)
                ReadyStateUser();
        }

        if (0.0f < m_ReSetTime)
            m_ReSetTime -= Time.deltaTime;

        if (PhotonNetwork.CurrentRoom == null || PhotonNetwork.LocalPlayer == null) return;

        if (PhotonNetwork.IsMasterClient)
        {
            Game_Mgr.Inst.UpdateKillCount(PlayerId, m_KillCount);
        }

    }
    public void ReadyStateUser()
    {
        if (Ready_Mgr.m_GameState != GameState.Ready) return;

        StartCoroutine(this.WaitReadyUser());
    }

    IEnumerator WaitReadyUser()
    {
        m_Canvas.enabled = false;
        Game_Mgr.Inst.m_GameObj.SetActive(false);

        SetVisible(false);

        //게임 시작 전까지 대기
        while (Ready_Mgr.m_GameState == GameState.Ready)
        {
            yield return null;
        }


        float pos = Random.Range(-10.0f, 10.0f);
        Vector3 a_SitPos = new Vector3(pos, 4.0f, pos);

        string a_TeamKind = ReceiveSelTeam(pv.Owner);
        int a_SitPosIdx = ReceiveSitPosIdx(pv.Owner);

        //팀에 따라 다른 위치에 앉게 하기
        if (0 <= a_SitPosIdx && a_SitPosIdx < 4)
        {
            //블루팀
            if (a_TeamKind == "blue")
            {
                a_SitPos = Ready_Mgr.m_Team1Pos[a_SitPosIdx];
            }
            //레드팀
            else if (a_TeamKind == "red")
            {
                a_SitPos = Ready_Mgr.m_Team2Pos[a_SitPosIdx];
            }
        }

        //플레이어의 위치를 변경
        this.gameObject.transform.position = a_SitPos;

        if (pv != null && pv.IsMine == true && m_HPBar !=null)
        {

            m_HPBar.fillAmount = 1.0f;
            m_HPBar.color = Color.green;

            m_CurHP = InitHp;

            m_Canvas.enabled = true;
            Game_Mgr.Inst.m_GameObj.SetActive(true);

        }


        SetVisible(true);

        m_StPos = a_SitPos;
        m_StCount = 5;

    }

    void SetVisible(bool IsVisible)
    {

        if (IsVisible == true)
            m_ReSetTime = 10.0f;
    }

    void OnTriggerEnter(Collider coll)
    {
        if (coll.tag == "Bullet" && m_CurHP > 0 && coll.tag == "Explosive")
        {
            int a_Att_Id = -1;//공격자 아이디
            string a_Att_Team = "blue";//공격자 팀
            int a_Dmg = 10;//데미지
            Bullet_Ctrl BulletSc = coll.gameObject.GetComponent<Bullet_Ctrl>();

            if (BulletSc != null)
            {
                a_Att_Id = BulletSc.AttackerId;
                a_Att_Team = BulletSc.AttackerTeam;
                a_Dmg = BulletSc.damage;
            }

            TakeDamage(a_Att_Id, a_Att_Team, a_Dmg);
        }

    }

    public void TakeDamage(int AttackerId = -1, string a_AttTeam = "blue", int a_Dmg = 10)
    {
        // 본인이 공격한 경우 데미지를 주지 않는다.
        if (AttackerId == PlayerId) return;

        // 0이 더 크면 데미지 제외
        if (m_CurHP <= 0) return;

        // 타이머 작동 중일 경우
        if (0 < m_ReSetTime) return;

        // 팀 판별
        string a_DamageTeam = "blue"; // 초기화
        if (pv.Owner.CustomProperties.ContainsKey("MyTeam"))
            a_DamageTeam = (string)pv.Owner.CustomProperties["MyTeam"];

        // 팀이 같은 경우 데미지 제외
        if (a_AttTeam == a_DamageTeam) return;

        m_Cur_LAttId = AttackerId;

        m_CurHP -= a_Dmg;

        Game_Mgr.Inst.ShowDamagePanel();

        if (m_CurHP < 0)
            m_CurHP = 0;

        UpdateHPBar();

        if (m_CurHP <= 0)
        {
            StartCoroutine(this.ExplosionUser());
        }

        // 로컬 플레이어의 HP를 동기화합니다.
        pv.RPC("SyncDamage", RpcTarget.All, m_CurHP, a_DamageTeam);
    }

    [PunRPC]
    void SyncDamage(int newHp, string a_DamageTeam)
    {
        m_CurHP = newHp;
        UpdateHPBar();

        if (m_CurHP <= 0)
        {
            StartCoroutine(this.ExplosionUser());
        }
    }

    void UpdateHPBar()
    {
        if (pv.IsMine && m_HPBar != null)
        {
            m_HPBar.fillAmount = (float)m_CurHP / (float)InitHp;

            if (m_HPBar.fillAmount <= 0.2f)
                m_HPBar.color = Color.red;
            else if (m_HPBar.fillAmount <= 0.6f)
                m_HPBar.color = Color.yellow;
            else
                m_HPBar.color = Color.green;
        }
    }

    IEnumerator ExplosionUser()
    {
        GameObject _Eff = GameObject.Instantiate(_Effect, transform.position, Quaternion.identity);

        Destroy(_Eff, 2.0f);

        m_Canvas.enabled = false;

        //hp바 숨기기
        Game_Mgr.Inst.m_GameObj.SetActive(false);

        SetVisible(false);


        // m_ReSetTime을 10으로 설정
        m_ReSetTime = 10.0f;

        yield return new WaitForSeconds(2.0f); // 폭발 후 2초 대기

        // HP 초기화
        m_CurHP = InitHp;
        UpdateHPBar();

        // 플레이어 위치 초기화
        this.gameObject.transform.position = m_StPos;

        // 캔버스와 게임 오브젝트 다시 활성화
        m_Canvas.enabled = true;
        Game_Mgr.Inst.m_GameObj.SetActive(true);

        SetVisible(true);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(m_CurHP);
            stream.SendNext(m_KillCount);
            stream.SendNext(m_Cur_LAttId);
        }
        else
        {
            NetHP = (int)stream.ReceiveNext();
            m_KillCount = (int)stream.ReceiveNext();
            m_Cur_LAttId = (int)stream.ReceiveNext();
        }
    }

    #region Photon Custom Properties Set
    string ReceiveSelTeam(Player a_Player)
    {
        string a_TeamKind = "blue";

        if (a_Player == null)
            return a_TeamKind;

        if (a_Player.CustomProperties.ContainsKey("MyTeam") == true)
        {
            a_TeamKind = (string)a_Player.CustomProperties["MyTeam"];
        }

        return a_TeamKind;
    }

    int ReceiveSitPosIdx(Player a_Player)
    {
        int a_SitIdx = -1;

        if (a_Player == null) return a_SitIdx;

        if (a_Player.CustomProperties.ContainsKey("SitPosInx") == true)
            a_SitIdx = (int)a_Player.CustomProperties["SitPosInx"];

        return a_SitIdx;
    }
    #endregion
}
