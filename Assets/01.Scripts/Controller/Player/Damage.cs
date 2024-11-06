using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Damage : MonoBehaviourPunCallbacks, IPunObservable
{
    public GameObject[] Objects;

    GameObject expEffect = null;

    #region HP
    int initHp = 440;
    [HideInInspector] public int currHp = 440;
    int NetHp = 440;
    Image hpBar;//Private인 이유는 다른 유저도 Game_Mgr의 HPBar를 사용하기 때문에
    #endregion

    public Canvas hudCanvas;

    #region KillCount
    [Header("killCount")]
    public Text txtKillCount;
    [HideInInspector] public int PlayerId = -1;
    int m_KillCount = 0;    
    int m_Cur_LAttId = -1;  
    ExitGames.Client.Photon.Hashtable KillProps =
                                        new ExitGames.Client.Photon.Hashtable();
    #endregion

    [HideInInspector] public float m_ReSetTime = 0.0f;

    #region Photon
    int m_StCount = 0;
    Vector3 m_StPos = Vector3.zero;
    PhotonView pv;
    #endregion


    void Start()
    {
        pv = GetComponent<PhotonView>();
        PlayerId = pv.Owner.ActorNumber;


        Objects = GetComponentsInChildren<GameObject>();

        //HP 연결 및 초기화
        currHp = (int)Game_Mgr.Inst.m_CurHP;
        initHp = (int)Game_Mgr.Inst.m_MaxHP;
        currHp = initHp;
        hpBar = Game_Mgr.Inst.m_HPBar;
        hpBar.color = Color.green;

        //폭발 효과 로드
        expEffect = Resources.Load<GameObject>("ExplosionMobile");

        //CustomProperties 초기화
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

        if (0.0f < m_ReSetTime) m_ReSetTime -= Time.deltaTime;

        if (PhotonNetwork.CurrentRoom == null ||PhotonNetwork.LocalPlayer == null) return;

        //다른 유저들 정보 동기화
        if (pv.IsMine == false)
        {
            AvataUpdate();

            ReceiveKillCount();
        }

        //킬카운트 표시
        if (txtKillCount != null)
            txtKillCount.text = m_KillCount.ToString();
    }

    public void ReadyStateUser()
    {
        if (Ready_Mgr.m_GameState != GameState.Ready) return;

        StartCoroutine(this.WaitReadyUser());
    }


    IEnumerator WaitReadyUser()
    {

        hudCanvas.enabled = false;
        Game_Mgr.Inst.m_GameObj.SetActive(false);


        SetVisible(false);

        while (Ready_Mgr.m_GameState == GameState.Ready)
        {
            yield return null;
        }

   
        float pos = Random.Range(-20.0f, 20.0f);
        Vector3 a_SitPos = new Vector3(pos, 20.0f, pos);

        string a_TeamKind = ReceiveSelTeam(pv.Owner);
        int a_SitPosInx = ReceiveSitPosInx(pv.Owner);


        if (0 <= a_SitPosInx && a_SitPosInx < 4)
        {
            //블루팀 위치 설정
            if (a_TeamKind == "blue")
            {
                a_SitPos = Ready_Mgr.m_Team1Pos[a_SitPosInx];
                this.gameObject.transform.eulerAngles = new Vector3(0.0f, 201.0f, 0.0f);
            }

            //레드팀 위치 설정
            else if (a_TeamKind == "red")
            {
                a_SitPos = Ready_Mgr.m_Team2Pos[a_SitPosInx];
                this.gameObject.transform.eulerAngles = new Vector3(0.0f, 19.5f, 0.0f);
            }

        }

        this.gameObject.transform.position = a_SitPos;


        hpBar.fillAmount = 1.0f;
        hpBar.color = Color.green;

        hudCanvas.enabled = true;
        Game_Mgr.Inst.m_GameObj.SetActive(true);

        if (pv != null && pv.IsMine == true)
            currHp = initHp;

        SetVisible(true);

        m_StPos = a_SitPos;
        m_StCount = 5;

    }

    void OnTriggerEnter(Collider coll)
    {
        //충돌한 Collider의 태크 비교
        if (currHp > 0 && coll.tag == "Bullet")
        {
            int a_Att_Id = -1;
            string a_AttTeam = "blue";
            Bullet_Ctrl a_RefCannon = coll.gameObject.GetComponent<Bullet_Ctrl>();
            if (a_RefCannon != null)
            {
                a_Att_Id = a_RefCannon.AttackerId;
                a_AttTeam = a_RefCannon.AttackerTeam;
            }

            TakeDamage(a_Att_Id, a_AttTeam);
        }
    }

    public void TakeDamage(int AttackerId = -1, string a_AttTeam = "blue")
    {
        if (AttackerId == PlayerId) return;

        if (currHp <= 0.0f) return;


        if (pv.IsMine == false) 
            return;

        if (0.0f < m_ReSetTime) return;

        string a_DamageTeam = "blue";
        if (pv.Owner.CustomProperties.ContainsKey("MyTeam") == true)
            a_DamageTeam = (string)pv.Owner.CustomProperties["MyTeam"];

        if (a_AttTeam == a_DamageTeam)
            return;


        m_Cur_LAttId = AttackerId;

        currHp -= 20;

        if (currHp < 0)
            currHp = 0;

        //현재 생명치 백분율 = (현재 생명치) / (초기 생명치)
        hpBar.fillAmount = (float)currHp / (float)initHp;

        //생명 수치에 따라 Filled 이미지의 색상을 변경
        if (hpBar.fillAmount <= 0.4f)
            hpBar.color = Color.red;
        else if (hpBar.fillAmount <= 0.6f)
            hpBar.color = Color.yellow;
        else
            hpBar.color = Color.green;

        if (currHp <= 0)  //죽는 처리 (아바타 탱크들은 중계 받아서 처리)
        {
            //IsMine 기준에서 죽는 처리
            StartCoroutine(this.ExplosionTank());
        }
    }

    //폭발 효과 생성 및 리스폰 코루틴 함수
    IEnumerator ExplosionTank()
    {
        //폭발 효과 생성
        GameObject effect = GameObject.Instantiate(expEffect,transform.position,Quaternion.identity);
        
        Destroy(effect, 3.0f);//3초후 폭발 효과 삭제

        //UI 비활성화
        hudCanvas.enabled = false;
        Game_Mgr.Inst.m_GameObj.SetActive(false);

        //사망 전 안보이게 처리
        SetVisible(false);

        yield return null;
    }

    //자식 오브젝트들을 모두 보이지 않게 처리하는 함수
    void SetVisible(bool isVisible)
    {
        Objects = GetComponentsInChildren<GameObject>();

        //자식 오브젝트들을 모두 보이지 않게 처리
        foreach (GameObject objs in Objects)
        {
            objs.SetActive(isVisible);
        }

        if (isVisible == true)
            m_ReSetTime = 10.0f;//10초간 딜레이 
    }

    //플레이어의 정보를 송수신
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(m_Cur_LAttId);
            stream.SendNext(currHp);
        }
        else
        {
            m_Cur_LAttId = (int)stream.ReceiveNext();
            NetHp = (int)stream.ReceiveNext();
        }
    }

    void AvataUpdate()
    {
        if (0 < currHp)
        {
            //다른 유저 HP 동기화
            currHp = NetHp;

            //HP바 갱신
            hpBar.fillAmount = (float)currHp / (float)initHp;

            //상태에 따른 색상변경
            if (hpBar.fillAmount <= 0.4f)
                hpBar.color = Color.red;

            else if (hpBar.fillAmount <= 0.6f)
                hpBar.color = Color.yellow;

            else
                hpBar.color = Color.green;

            if (currHp <= 0)
            {
                currHp = 0;

                if (0 <= m_Cur_LAttId)
                {
                    SaveKillCount(m_Cur_LAttId);
                }

                StartCoroutine(this.ExplosionTank());
            }

        }
        else
        {
            currHp = NetHp;
            if ((int)(initHp * 0.95f) < currHp)
            {

                hpBar.fillAmount = 1.0f;

                hpBar.color = Color.green;

                //UI활성화
                hudCanvas.enabled = true;
                Game_Mgr.Inst.m_GameObj.SetActive(true);

                //리스폰 시 HP 초기화
                currHp = initHp;

                //다시 보이게 처리
                SetVisible(true);

            }
        }

    }


    void SaveKillCount(int AttackerId)
    {

        GameObject[] users = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject user in users)
        {
            var a_Damage = user.GetComponent<Damage>();
            if (a_Damage != null && a_Damage.PlayerId == AttackerId)
            {

                if (a_Damage.IncKillCount() == true)
                {
                    return;
                }
            }
        }
    }

    //킬 카운트 증가 함수
    public bool IncKillCount()
    {
        //pv.IsMine 일 때만 증가
        if (pv != null && pv.IsMine == true)
        {
            m_KillCount++;

            //KillCount를 원격지 유저들에게 전송
            SendKillCount(m_KillCount);

            return true;
        }

        return false;//원격지 유저들은 증가하지 않음(중계만 받음)
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
        if (pv == null) return;

        if (pv.IsMine == false) return;

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

    //킬 카운트 수신
    void ReceiveKillCount()
    {
        if (pv == null) return; //PhotonView 컴포넌트가 없으면 리턴

        //원격지 탱크(아바타)일 때 수신받는거라 걸리지않아야함.
        if (pv.IsMine == true) return;

        if (pv.Owner == null) return;

        //killCount를 수신받아서 m_KillCount 변수에 저장
        if (pv.Owner.CustomProperties.ContainsKey("KillCount") == true)
        {
            m_KillCount = (int)pv.Owner.CustomProperties["KillCount"];
        }
    }

    //팀 선택 정보 수신
    string ReceiveSelTeam(Player a_Player)
    {
        string a_TeamKind = "blue";

        if (a_Player == null)
            return a_TeamKind;

        if (a_Player.CustomProperties.ContainsKey("MyTeam") == true)
            a_TeamKind = (string)a_Player.CustomProperties["MyTeam"];

        return a_TeamKind;
    }

    //팀 위치 정보 수신
    int ReceiveSitPosInx(Player a_Player)
    {
        int a_SitIdx = -1;

        if (a_Player == null) return a_SitIdx;

        if (a_Player.CustomProperties.ContainsKey("SitPosInx") == true)
            a_SitIdx = (int)a_Player.CustomProperties["SitPosInx"];

        return a_SitIdx;
    }
}