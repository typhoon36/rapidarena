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

        //--- 원격 플레이어(아바타 탱크 입장) 일 때 동기화 코드
        if (pv.IsMine == false)
        { //원격 플레이어(아바타 탱크 입장) 일 때 수행
            AvataUpdate();

            ReceiveKillCount(); //아바타 탱크들 입장에서 KillCount 중계 방아옴
        }
        //--- 원격 플레이어(아바타 탱크 입장) 일때 동기화 코드

        if (m_KillTxt != null)
            m_KillTxt.text = m_KillCount.ToString(); //킬 카운트 UI 갱신
    }

    public void ReadyState()
    {
        if (Ready_Mgr.m_GameState != GameState.Ready) return;

        StartCoroutine(WaitReady());
    }

    IEnumerator WaitReady()
    {
        //HUD를 비활성화
        m_Canvas.enabled = false;

        //탱크 투명 처리
        SetTankVisible(false);

        while (Ready_Mgr.m_GameState == GameState.Ready)
        {
            yield return null;
        }

        //탱크 특정한 위치에 리스폰 되도록...
        float pos = Random.Range(-100.0f, 100.0f);
        Vector3 a_SitPos = new Vector3(pos, 20.0f, pos);

        string a_TeamKind = ReceiveSelTeam(pv.Owner);   //자기 소속 팀 받아오기
        int a_SitPosInx = ReceiveSitPosInx(pv.Owner);   //자기 자리 번호 받아오기
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

        //탱크 특정한 위치에 리스폰 되도록...

        //Filled 이미지 초기값으로 환원
        //hpBar.fillAmount = 1.0f;
        //HUD 활성화
        m_Canvas.enabled = true;

        // if (pv != null && pv.IsMine == true)    //리스폰 시 생명 초기값 설정
        // currHp = initHp;

        //탱크를 다시 보이게 처리
        SetTankVisible(true);

        m_StPos = a_SitPos;
        m_StCount = 5;

    }//IEnumerator WaitReadyTank()

    void OnTriggerEnter(Collider coll)
    {
        //충돌한 Collider의 태크 비교
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
            // IsMine 기준에서 죽는 처리
    //        StartCoroutine(this.ExplosionTank());
      //      gameMgr.Death();
        //}
    }

    //폭발 효과 생성 및 리스폰 코루틴 함수
    IEnumerator ExplosionTank()
    {
        //폭발 효과 생성
        GameObject effect = GameObject.Instantiate(expEff,
                                            transform.position,
                                            Quaternion.identity);
        Destroy(effect, 3.0f);

        //HUD를 비활성화
        m_Canvas.enabled = false;

        //탱크 투명 처리
        SetTankVisible(false);

        yield return null;


    }

    //MeshRenderer를 활성화/비활성화하는 함수
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
        if (stream.IsWriting)  //IsMine쪽 정보 송신
        {
            stream.SendNext(m_Cur_LAttId); //이번에 -20 깍인 게 누구 때문인지 같이 보내준다는 의도
            //stream.SendNext(currHp);
        }
        else 
        {
            m_Cur_LAttId = (int)stream.ReceiveNext(); 
            //NetHp = (int)stream.ReceiveNext();
            //아바타 입장에서 사망 시점을 알기 위해 NetHp 라는 변수를 따로 만들어서
            //IsMine에서 송신해 준 Hp 값을 받았다.
        }
    }

    void AvataUpdate()  //원격지 플레이어 Hp Update 처리 함수
    {
        //if (0 < currHp)
        //{
        //    currHp = NetHp;

        //    //현재 생명치 백분율 = (현재 생명치) / (초기 생명치)
        //    hpBar.fillAmount = (float)currHp / (float)initHp;

        //    //생명 수치에 따라 Filled 이미지의 색상을 변경
        //    if (hpBar.fillAmount <= 0.4f)
        //        hpBar.color = Color.red;
        //    else if (hpBar.fillAmount <= 0.6f)
        //        hpBar.color = Color.yellow;
        //    else
        //        hpBar.color = Color.green;

        //    if (currHp <= 0) //죽는 처리 (아바타 탱크들은 중계 받아서 처리)
        //    {
        //        currHp = 0;

        //        if (0 <= m_Cur_LAttId)  //공격자 Id가 유효할 때
        //        { //지금 Hp가 깎이게 해서 사망에 이르게 한 탱크가 누구인지?
        //            // 지금 죽은 탱크 입장에서는
        //            // 공격자의 AttackerId (<--- IsMine)을 찾으려면 
        //            // 죽은 탱크 아바타들 중에서 AttackerId (<--- IsMine)을 찾아서
        //            // KillCount를 증가시켜 줘야 한다.
        //            // 자신을 파괴시킨 적 탱크의 스코어를 증가시키는 함수를 호출
        //            SaveKillCount(m_Cur_LAttId);
        //        }

        //        //IsMine 기준에서 죽는 처리
        //        StartCoroutine(this.ExplosionTank());
        //    }

        //} //if(0 < currHp)
        //else //if(currHp <= 0) 죽어 있는 상황에서
        //{ //죽어 있을 때 계속 NetHp는 0으로 계속 들어오게 되고
        //    //되살려야 하는 상황 처리
        //    currHp = NetHp;
        //    if ((int)(initHp * 0.95f) < currHp) //이번에 들어온 Hp가 최대 에너지가 들어오면
        //    {   //되살려야 하는 상황으로 판단하겠다는 뜻

        //        //Filled 이미지 초기값으로 환원
        //        hpBar.fillAmount = 1.0f;
        //        //Filled 이미지 색상을 녹색으로 설정
        //        hpBar.color = Color.green;
        //        //HUD 활성화
        //        hudCanvas.enabled = true;

        //        //리스폰 시 새 생명 초기값 설정
        //        currHp = initHp;
        //        //탱크를 다시 보이게 처리
        //        SetTankVisible(true);

        //    }//if ((int)(initHp * 0.95f) < currHp) //이번에 들어온 Hp가 최대 에너지가 들어오면
        //}//else //if(currHp <= 0) 죽어 있는 상황에서

    }

    //자신을 파괴시킨 적 탱크를 검색해 스코어를 증가시키는 함수
    void SaveKillCount(int AttacketId)
    {
        //TANL 테크로 지정된 모든 탱크를 가져와 배열에 저장
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