using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Damage : MonoBehaviourPunCallbacks, IPunObservable
{

    GameObject expEff;

    int NetHP = 440;

    public Canvas m_Canvas;

    PhotonView pv;

    public Text m_KillTxt;

    [HideInInspector] public int PlayerId = -1;

    int m_KillCount = 0;
    int m_Cur_LAttId = -1;

    ExitGames.Client.Photon.Hashtable KillProps =
                                        new ExitGames.Client.Photon.Hashtable();
    [HideInInspector] public float m_ReSetTime = 0.0f;   

    int m_StCount = 0;
    Vector3 m_StPos = Vector3.zero;

    void Start()
    {
        pv = GetComponent<PhotonView>();

        PlayerId = pv.Owner.ActorNumber;

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

            }
        }



        if (0.0f < m_ReSetTime)
            m_ReSetTime -= Time.deltaTime;

        if (PhotonNetwork.CurrentRoom == null ||
            PhotonNetwork.LocalPlayer == null)
            return;

        //--- ���� �÷��̾�(�ƹ�Ÿ ��ũ ����) �� �� ����ȭ �ڵ�
        if (pv.IsMine == false)
        { //���� �÷��̾�(�ƹ�Ÿ ��ũ ����) �� �� ����
            AvataUpdate();

            ReceiveKillCount(); //�ƹ�Ÿ ��ũ�� ���忡�� KillCount �߰� ��ƿ�
        }
        //--- ���� �÷��̾�(�ƹ�Ÿ ��ũ ����) �϶� ����ȭ �ڵ�

        if (m_KillTxt != null)
            m_KillTxt.text = m_KillCount.ToString(); //ų ī��Ʈ UI ����
    }

    public void ReadyState()
    {
        if (Ready_Mgr.m_GameState != GameState.Ready) return;

        StartCoroutine(WaitReady());
    }

    IEnumerator WaitReady()
    {
        //HUD�� ��Ȱ��ȭ
        m_Canvas.enabled = false;

        //��ũ ���� ó��
        SetTankVisible(false);

        while (Ready_Mgr.m_GameState == GameState.Ready)
        {
            yield return null;
        }

        //��ũ Ư���� ��ġ�� ������ �ǵ���...
        float pos = Random.Range(-100.0f, 100.0f);
        Vector3 a_SitPos = new Vector3(pos, 20.0f, pos);

        string a_TeamKind = ReceiveSelTeam(pv.Owner);   //�ڱ� �Ҽ� �� �޾ƿ���
        int a_SitPosInx = ReceiveSitPosInx(pv.Owner);   //�ڱ� �ڸ� ��ȣ �޾ƿ���
        if (0 <= a_SitPosInx && a_SitPosInx < 4)
        {
            if (a_TeamKind == "blue")
            {
                a_SitPos = Ready_Mgr.m_Team1Pos[a_SitPosInx];
                this.gameObject.transform.eulerAngles =
                                    new Vector3(0.0f, 201.0f, 0.0f);
            }
            else if (a_TeamKind == "red")
            {
                a_SitPos = Ready_Mgr.m_Team2Pos[a_SitPosInx];
                this.gameObject.transform.eulerAngles =
                                    new Vector3(0.0f, 19.5f, 0.0f);
            }
        }//if(0 <= a_SitPosInx && a_SitPosInx < 4)

        this.gameObject.transform.position = a_SitPos;

        //��ũ Ư���� ��ġ�� ������ �ǵ���...

        //Filled �̹��� �ʱⰪ���� ȯ��
        //hpBar.fillAmount = 1.0f;
        //HUD Ȱ��ȭ
        m_Canvas.enabled = true;

        // if (pv != null && pv.IsMine == true)    //������ �� ���� �ʱⰪ ����
        // currHp = initHp;

        //��ũ�� �ٽ� ���̰� ó��
        SetTankVisible(true);

        m_StPos = a_SitPos;
        m_StCount = 5;

    }//IEnumerator WaitReadyTank()

    void OnTriggerEnter(Collider coll)
    {
        //�浹�� Collider�� ��ũ ��
        //if (currHp > 0 && coll.tag == "Bullet")
        //{
        //    int a_Att_Id = -1;
        //    string a_AttTeam = "blue";
        //    Bullet_Ctrl a_RefBullet = coll.gameObject.GetComponent<Bullet_Ctrl>();
        //    if (a_RefBullet != null)
        //    {
        //        a_Att_Id = a_RefBullet.AttackerId;
        //        a_AttTeam = a_RefBullet.AttackerTeam;
        //    }

        //    TakeDamage(a_Att_Id, a_AttTeam);


        //}
    }
    public void TakeDamage(int AttackerId = -1, string a_AttTeam = "blue")
    {
        if (AttackerId == PlayerId)
            return;

        //if (currHp <= 0.0f)
        //    return;

        if (pv.IsMine == false)
            return;

        if (0.0f < m_ReSetTime)
            return;

        string a_DamageTeam = "blue";
        if (pv.Owner.CustomProperties.ContainsKey("MyTeam") == true)
            a_DamageTeam = (string)pv.Owner.CustomProperties["MyTeam"];

        if (a_AttTeam == a_DamageTeam)
            return;

        m_Cur_LAttId = AttackerId;
        //currHp -= 20;
        //if (currHp < 0)
          //  currHp = 0;


//        if (currHp <= 0)
  //      {
            // IsMine ���ؿ��� �״� ó��
    //        StartCoroutine(this.ExplosionTank());
      //      gameMgr.Death();
        //}
    }

    //���� ȿ�� ���� �� ������ �ڷ�ƾ �Լ�
    IEnumerator ExplosionTank()
    {
        //���� ȿ�� ����
        GameObject effect = GameObject.Instantiate(expEff,
                                            transform.position,
                                            Quaternion.identity);
        Destroy(effect, 3.0f);

        //HUD�� ��Ȱ��ȭ
        m_Canvas.enabled = false;

        //��ũ ���� ó��
        SetTankVisible(false);

        yield return null;


    }

    //MeshRenderer�� Ȱ��ȭ/��Ȱ��ȭ�ϴ� �Լ�
    void SetTankVisible(bool isVisible)
    {
        //foreach (MeshRenderer _renderer in renderers)
        //{
        //    _renderer.enabled = isVisible;
        //}

        //Rigidbody[] a_Rigs = GetComponentsInChildren<Rigidbody>(true);
        //foreach (Rigidbody _Rigd in a_Rigs)
        //{
        //    _Rigd.isKinematic = !isVisible;
        //}

        //BoxCollider[] a_BoxColls = this.GetComponentsInChildren<BoxCollider>(true);
        //foreach (BoxCollider _BoxColl in a_BoxColls)
        //{
        //    _BoxColl.enabled = isVisible;
        //}

        if (isVisible == true)
            m_ReSetTime = 10.0f;

    }//void SetTankVisible(bool isVisible)

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)  //IsMine�� ���� �۽�
        {
            stream.SendNext(m_Cur_LAttId); //�̹��� -20 ���� �� ���� �������� ���� �����شٴ� �ǵ�
            //stream.SendNext(currHp);
        }
        else 
        {
            m_Cur_LAttId = (int)stream.ReceiveNext(); 
            //NetHp = (int)stream.ReceiveNext();
            //�ƹ�Ÿ ���忡�� ��� ������ �˱� ���� NetHp ��� ������ ���� ����
            //IsMine���� �۽��� �� Hp ���� �޾Ҵ�.
        }
    }

    void AvataUpdate()  //������ �÷��̾� Hp Update ó�� �Լ�
    {
        //if (0 < currHp)
        //{
        //    currHp = NetHp;

        //    //���� ����ġ ����� = (���� ����ġ) / (�ʱ� ����ġ)
        //    hpBar.fillAmount = (float)currHp / (float)initHp;

        //    //���� ��ġ�� ���� Filled �̹����� ������ ����
        //    if (hpBar.fillAmount <= 0.4f)
        //        hpBar.color = Color.red;
        //    else if (hpBar.fillAmount <= 0.6f)
        //        hpBar.color = Color.yellow;
        //    else
        //        hpBar.color = Color.green;

        //    if (currHp <= 0) //�״� ó�� (�ƹ�Ÿ ��ũ���� �߰� �޾Ƽ� ó��)
        //    {
        //        currHp = 0;

        //        if (0 <= m_Cur_LAttId)  //������ Id�� ��ȿ�� ��
        //        { //���� Hp�� ���̰� �ؼ� ����� �̸��� �� ��ũ�� ��������?
        //            // ���� ���� ��ũ ���忡����
        //            // �������� AttackerId (<--- IsMine)�� ã������ 
        //            // ���� ��ũ �ƹ�Ÿ�� �߿��� AttackerId (<--- IsMine)�� ã�Ƽ�
        //            // KillCount�� �������� ��� �Ѵ�.
        //            // �ڽ��� �ı���Ų �� ��ũ�� ���ھ ������Ű�� �Լ��� ȣ��
        //            SaveKillCount(m_Cur_LAttId);
        //        }

        //        //IsMine ���ؿ��� �״� ó��
        //        StartCoroutine(this.ExplosionTank());
        //    }

        //} //if(0 < currHp)
        //else //if(currHp <= 0) �׾� �ִ� ��Ȳ����
        //{ //�׾� ���� �� ��� NetHp�� 0���� ��� ������ �ǰ�
        //    //�ǻ���� �ϴ� ��Ȳ ó��
        //    currHp = NetHp;
        //    if ((int)(initHp * 0.95f) < currHp) //�̹��� ���� Hp�� �ִ� �������� ������
        //    {   //�ǻ���� �ϴ� ��Ȳ���� �Ǵ��ϰڴٴ� ��

        //        //Filled �̹��� �ʱⰪ���� ȯ��
        //        hpBar.fillAmount = 1.0f;
        //        //Filled �̹��� ������ ������� ����
        //        hpBar.color = Color.green;
        //        //HUD Ȱ��ȭ
        //        hudCanvas.enabled = true;

        //        //������ �� �� ���� �ʱⰪ ����
        //        currHp = initHp;
        //        //��ũ�� �ٽ� ���̰� ó��
        //        SetTankVisible(true);

        //    }//if ((int)(initHp * 0.95f) < currHp) //�̹��� ���� Hp�� �ִ� �������� ������
        //}//else //if(currHp <= 0) �׾� �ִ� ��Ȳ����

    }

    //�ڽ��� �ı���Ų �� ��ũ�� �˻��� ���ھ ������Ű�� �Լ�
    void SaveKillCount(int AttacketId)
    {
        //TANL ��ũ�� ������ ��� ��ũ�� ������ �迭�� ����
        GameObject[] users = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject user in users)
        {
            var a_Damage = user.GetComponent<Damage>();
            if (a_Damage != null && a_Damage.PlayerId == AttacketId)
            { 
                if (a_Damage.IncKillCount() == true)
                {
                    return;
                }
            }
        }
    }

    public bool IncKillCount() 
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
        if (pv == null)
            return;

        if (pv.IsMine == false) 
            return;

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
        if (pv == null)
            return;

        if (pv.IsMine == true)   
            return;

        if (pv.Owner == null)
            return;

        if (pv.Owner.CustomProperties.ContainsKey("KillCount") == true)
        {
            m_KillCount = (int)pv.Owner.CustomProperties["KillCount"];
        }
    }

    string ReceiveSelTeam(Player a_Player) 
    {
        string a_TeamKind = "blue";

        if (a_Player == null)
            return a_TeamKind;

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