using Photon.Pun;
using Photon.Pun.Demo.Asteroids;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Damage : MonoBehaviourPunCallbacks, IPunObservable
{
    MeshRenderer[] m_Rends;

    GameObject _Effect;

    int InitHp = 400;
    [HideInInspector] public int m_CurHP = 400;
    int NetHP = 400;


    public Canvas m_Canvas;
    [HideInInspector] public int PlayerId = -1;//�÷��̾� ���̵�(������ȣ)


    Image m_HPBar;
    //Private�� ������ Game_Mgr���� ������ �������� ������ֱ� ����.

    #region Photon
    PhotonView pv;
    int m_KillCount = 0;
    int m_Cur_LAttId = -1;

    [HideInInspector] public float m_ReSetTime = 0.0f;

    [HideInInspector] public int m_StCount = 0;
    Vector3 m_StPos = Vector3.zero;
    #endregion

    void Start()
    {
        pv = GetComponent<PhotonView>();
        PlayerId = pv.Owner.ActorNumber;//�÷��̾ ������ ���̵� �����´�.

        _Effect = Resources.Load<GameObject>("ExplosionMobile");//�׾������� ����Ʈ �ε�

        m_Rends = GetComponentsInChildren<MeshRenderer>();//ĸ���� ������ ��������

        //�÷��̾�� �� HP �ʱ�ȭ
        //true�� ������ fps���� �÷��̾���� m_HPBar�� ������ ����ϱ� ����
        if (pv.IsMine == true)
        {
            InitHp = m_CurHP;//�ʱ�ȭ
            m_HPBar = Game_Mgr.Inst.m_HPBar;
            m_HPBar.fillAmount = 1.0f;
            m_HPBar.color = Color.green;
        }
    }

    int m_UpdateCk = 2;
    void Update()
    {
        //�������� �ʱ�ȭ
        if (0 < m_UpdateCk)
        {
            //�÷��̾��� ĸ���� �������ʰ� �Ͽ� �����
            m_UpdateCk--;
            if (m_UpdateCk <= 0)
                ReadyStateUser();
        }

        if (0.0f < m_ReSetTime)
            m_ReSetTime -= Time.deltaTime;

        if (PhotonNetwork.CurrentRoom == null || PhotonNetwork.LocalPlayer == null) return;
    }

    public void ReadyStateUser()
    {
        if (Ready_Mgr.m_GameState != GameState.Ready) return;

        m_CurHP = InitHp;
        UpdateHPBar();

        StartCoroutine(this.WaitReadyUser());
    }

    //���� ���� ������ ���
    IEnumerator WaitReadyUser()
    {
        //�������̴� ��� �����
        m_Canvas.enabled = false;

        SetVisible(false);

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
        if (0 <= a_SitPosIdx && a_SitPosIdx < 2)
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

        if (pv != null && pv.IsMine == true && m_HPBar !=null)
        {
            m_HPBar.fillAmount = 1.0f;
            m_HPBar.color = Color.green;
            m_CurHP = InitHp;

            Game_Mgr.Inst.m_GameObj.SetActive(true);

        }


        m_Canvas.enabled = true;

        SetVisible(true);

        m_StPos = a_SitPos;
        m_StCount = 5;
    }

    void SetVisible(bool IsVisible)
    {
        foreach (MeshRenderer a_Rend in m_Rends)
        {
            a_Rend.enabled = IsVisible;
        }

        if (IsVisible == true)
            m_ReSetTime = 10.0f;
    }

    #region Damage
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

            TakeDamage(a_Att_Id, a_Att_Team, a_Dmg);
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

        // ���� ���� ��� ������ ����
        if (a_AttTeam == a_DamageTeam) return;

        m_Cur_LAttId = AttackerId;

        m_CurHP -= a_Dmg;

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

            if (m_HPBar.fillAmount <= 0.2f)
                m_HPBar.color = Color.red;
            else if (m_HPBar.fillAmount <= 0.6f)
                m_HPBar.color = Color.yellow;
            else
                m_HPBar.color = Color.green;
        }
    }
    #endregion

    IEnumerator ExplosionUser()
    {
        GameObject _Eff = GameObject.Instantiate(_Effect, transform.position, Quaternion.identity);

        Destroy(_Eff, 3.0f);//����Ʈ ����

        if (pv.IsMine == true)
        {
            m_Canvas.enabled = false;//�г��� ĵ���� �����

            Game_Mgr.Inst.m_GameObj.SetActive(false);//HP�� ������ ���� ������Ʈ �����

        }

        //����ó��
        SetVisible(false);

        transform.position = m_StPos;

        yield return null; // �� ������ ��� �� �ڷ�ƾ ���� �Լ� ����
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
