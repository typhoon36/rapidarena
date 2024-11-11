using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GunType { AssaultRifle = 0, Handgun, CombatKnife, Grenade }
public enum ThrowType { Default, Smoke }

[System.Serializable]
public class WeaponSetting
{
    public GunType name; // 총의 이름
    public ThrowType Throwname; // 수류탄 이름
    public int m_Damage; // 총의 데미지
    public float m_AttackRate;
    public float m_AttackDist;
    public bool IsAutoAttack;
    public int m_ClipSize;
    public int m_MaxAmmo;
}

// 모든 컨트롤러의 부모 클래스
public class Base_Ctrl : MonoBehaviourPunCallbacks
{

    public WeaponSetting weaponSetting;
    protected int m_CurrentAmmo;
    protected int m_AmmoInClip;

    public enum DefState
    {
        Idle,
        Jump,
        Walk,
        Run,
        Fire,
        Reload,
        Inspect,
        Damaged,
        Death
    }

    [HideInInspector] public Animator m_Anim;
    [SerializeField]
    protected DefState m_PlayerState = DefState.Idle;

    void Awake()
    {
        m_Anim = GetComponentInChildren<Animator>();
    }

    #region 플레이어 상태에 따른 애니메이션
    public virtual DefState PlayerState
    {
        get { return m_PlayerState; }
        set
        {
            if (m_PlayerState != value)
            {
                m_PlayerState = value;
                UpdateAnimationState();
            }
        }
    }

    public bool Aim
    {
        set
        {
            if (m_Anim != null)
                m_Anim.SetBool("Aim", value);
        }

        get => m_Anim != null && m_Anim.GetBool("Aim");
    }

    protected void UpdateAnimationState()
    {
        if (m_Anim == null || !m_Anim.gameObject.activeInHierarchy) return;

        switch (m_PlayerState)
        {
            case DefState.Idle:
                m_Anim.CrossFade("Idle", 0.2f);
                break;
            case DefState.Walk:
                m_Anim.CrossFade("Walk", 0.2f);
                break;
            case DefState.Run:
                m_Anim.CrossFade("Run", 0.2f);
                break;
            case DefState.Fire:
                m_Anim.CrossFade("Fire", 0.2f);
                break;
            case DefState.Reload:
                m_Anim.CrossFade("Reload", 0.2f);
                break;
            case DefState.Inspect:
                m_Anim.CrossFade("Inspect", 0.2f);
                break;
        }

        // 애니메이션 상태를 네트워크에 동기화
        if (photonView.IsMine)
        {
            photonView.RPC("RPC_UpdateAnimationState", RpcTarget.Others, m_PlayerState);
        }
    }

    [PunRPC]
    void RPC_UpdateAnimationState(DefState state)
    {
        m_PlayerState = state;
        UpdateAnimationState();
    }

    public virtual void Fire()
    {
        if (this is Player_Ctrl playerCtrl) // 플레이어 컨트롤러인 경우
        {
            string anim = playerCtrl.Aim ? "AimFire" : "Fire";
            playerCtrl.SetAnimation(anim);
        }
    }

    public virtual void Reload()
    {
        // 예외 처리
        if (m_PlayerState == DefState.Reload) return;
        if (m_PlayerState == DefState.Inspect) return;
        if (m_PlayerState == DefState.Jump) return;
        if (m_PlayerState == DefState.Run) return;
        if (m_PlayerState == DefState.Walk) return;

        // 리로드 상태로 전환
        PlayerState = DefState.Reload;
        SetTrigger("Reload"); // 애니메이션 트리거 설정
    }

    public virtual void Inspect()
    {
        PlayerState = DefState.Inspect;
    }

    public void SetAnimation(string animName)
    {
        if (m_Anim != null && m_Anim.gameObject.activeInHierarchy)
        {
            m_Anim.Play(animName);
        }
    }

    public void SetTrigger(string paramName)
    {
        if (m_Anim != null && m_Anim.gameObject.activeInHierarchy)
        {
            m_Anim.SetTrigger(paramName);
        }
    }
    #endregion -- 기본 상태처리


    #region 플레이어의 탄창 회복
    public void IncreaseMag(int mag)
    {
        Weapon_Base[] weapons = GetComponentsInChildren<Weapon_Base>();
        foreach (var weapon in weapons)
        {
            weapon.IncreaseAmmo(mag);
        }
    }
    #endregion


   




}
