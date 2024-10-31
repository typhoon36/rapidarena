using Photon.Pun;
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

    int m_KillCount = 0;
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
        //InitCustomProperties(pv);
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

    }
}
