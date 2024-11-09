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

    #region ųī��Ʈ 
    [Header("KillCount")]
    public Canvas m_Canvas;//killtext�� ���� ���� ĵ����
    public Text m_KillText;//ųī��Ʈ�� ���� ���� �ؽ�Ʈ
    [HideInInspector] public int PlayerId = -1;
    #endregion

    Image m_HPBar;
    //Private�� ������ Game_Mgr���� ������ �������� ������ֱ� ����.

    #region Photon
    PhotonView pv;
    int m_KillCount = 0;
    int m_Cur_LAttId = -1;
    ExitGames.Client.Photon.Hashtable KillProps =
                                        new ExitGames.Client.Photon.Hashtable();

    [HideInInspector] public float m_ReSetTime = 0.0f;

    int m_StCount = 0;
    Vector3 m_StPos = Vector3.zero;
    #endregion

    void Start()
    {
        pv = GetComponent<PhotonView>();
        PlayerId = pv.Owner.ActorNumber;//Owner: �ش� ������Ʈ�� ������ �÷��̾�

        m_CurHP = (int)Game_Mgr.Inst.m_CurHP;
        InitHp = (int)Game_Mgr.Inst.m_MaxHP;
        InitHp = m_CurHP;//�ʱ�ȭ

        if (pv.IsMine == true)
        {
            m_HPBar = Game_Mgr.Inst.m_HPBar;
            m_HPBar.fillAmount = 1.0f;
            m_HPBar.color = Color.green;
        }

        _Effect = Resources.Load<GameObject>("ExplosionMobile");

        //custom property
        InitCustomProperties(pv);
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

        if (m_KillText != null)
            m_KillText.text = m_KillCount.ToString();

    }
    public void ReadyStateUser()
    {
        if (Ready_Mgr.m_GameState != GameState.Ready)
            return;

        StartCoroutine(this.WaitReadyUser());
    }
    IEnumerator WaitReadyUser()
    {
        m_Canvas.enabled = false;
        Game_Mgr.Inst.m_GameObj.SetActive(false);

        //���� ���� ������ ���
        while (Ready_Mgr.m_GameState == GameState.Ready)
        {
            yield return null;
        }

        float pos = Random.Range(-10.0f, 10.0f);
        Vector3 a_SitPos = new Vector3(pos, 4.0f, pos);

        string a_TeamKind = ReceiveSelTeam(pv.Owner);
        int a_SitPosIdx = ReceiveSitPosIdx(pv.Owner);

        //���� ���� �ٸ� ��ġ�� �ɰ� �ϱ�
        if (0 <= a_SitPosIdx && a_SitPosIdx < 4)
        {
            //�����
            if (a_TeamKind == "blue")
            {
                a_SitPos = Ready_Mgr.m_Team1Pos[a_SitPosIdx];

            }
            //������
            else if (a_TeamKind == "red")
            {
                a_SitPos = Ready_Mgr.m_Team2Pos[a_SitPosIdx];


            }

        }

        //�÷��̾��� ��ġ�� ����
        this.gameObject.transform.position = a_SitPos;

        if (m_HPBar != null)
        {
            m_HPBar.fillAmount = 1.0f;
            m_HPBar.color = Color.green;
        }

        m_Canvas.enabled = true;
        Game_Mgr.Inst.m_GameObj.SetActive(true);

        if (pv != null && pv.IsMine == true)
            m_CurHP = InitHp;

        m_StPos = a_SitPos;
        m_StCount = 5;
    }

    void OnTriggerEnter(Collider coll)
    {
        if (coll.tag == "Bullet" && m_CurHP > 0)
        {
            int a_Att_Id = -1;//������ ���̵�
            string a_Att_Team = "blue";//������ ��
            int a_Dmg = 10;//������
            Bullet_Ctrl BulletSc = coll.gameObject.GetComponent<Bullet_Ctrl>();

            if (BulletSc != null)
            {
                a_Att_Id = BulletSc.AttackerId;
                a_Att_Team = BulletSc.AttackerTeam;
                a_Dmg = BulletSc.damage;
            }


            TakeDamage(a_Att_Id, a_Att_Team);
        }

    }

    public void TakeDamage(int AttackerId = -1, string a_AttTeam = "blue", int a_Dmg = 10)
    {
        // ������ ������ ��� �������� ���� �ʴ´�.
        if (AttackerId == PlayerId) return;

        // 0�� �� ũ�� ������ ����
        if (m_CurHP <= 0) return;

        // Ÿ�̸� �۵� ���� ���
        if (0 < m_ReSetTime) return;

        // �� �Ǻ�
        string a_DamageTeam = "blue"; // �ʱ�ȭ
        if (pv.Owner.CustomProperties.ContainsKey("MyTeam"))
            a_DamageTeam = (string)pv.Owner.CustomProperties["MyTeam"];


        // ����� �α� �߰�
        Debug.Log($"Attacker Team: {a_AttTeam}, Damage Team: {a_DamageTeam}");

        // ���� ���� ��� ������ ����
        if (a_AttTeam == a_DamageTeam) return;

        m_Cur_LAttId = AttackerId;

        m_CurHP -= a_Dmg;

        Debug.Log("TakeDamage : " + a_Dmg + " Team: " + a_AttTeam);

        if (m_CurHP < 0)
            m_CurHP = 0;

        UpdateHPBar();

        if (m_CurHP <= 0)
        {
            StartCoroutine(this.ExplosionUser());
        }

        // ���� �÷��̾��� HP�� ����ȭ�մϴ�.
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

            if (m_HPBar.fillAmount <= 0.4f)
                m_HPBar.color = Color.red;
            else if (m_HPBar.fillAmount <= 0.7f)
                m_HPBar.color = Color.yellow;
            else
                m_HPBar.color = Color.green;
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // ���� �÷��̾��� ���� HP�� �����մϴ�.
            stream.SendNext(m_CurHP);
            stream.SendNext(m_Cur_LAttId);
        }
        else
        {
            // ���� �÷��̾��� ���� HP�� �����մϴ�.
            NetHP = (int)stream.ReceiveNext();
            m_Cur_LAttId = (int)stream.ReceiveNext();

            // ���� �÷��̾��� HP�� ������Ʈ�մϴ�.
            if (!pv.IsMine)
            {
                m_CurHP = NetHP;
                UpdateHPBar();
            }
        }
    }

    IEnumerator ExplosionUser()
    {
        GameObject _Eff = GameObject.Instantiate(_Effect, transform.position, Quaternion.identity);

        Destroy(_Eff, 2.0f);

        m_Canvas.enabled = false;

        //hp�� �����
        Game_Mgr.Inst.m_GameObj.SetActive(false);

        yield return null;

    }

    //void SetVisible(bool a_IsVisible)
    //{
    //    if (pv.IsMine == true && a_IsVisible == true)
    //    {

    //    }
    //}


    #region Photon Custom Properties Set

    void InitCustomProperties(PhotonView pv)
    {
        if (pv != null && pv.IsMine == true)
        {
            KillProps.Clear();
            KillProps.Add("KillCount", 0);
            pv.Owner.SetCustomProperties(KillProps);
        }
    }
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
