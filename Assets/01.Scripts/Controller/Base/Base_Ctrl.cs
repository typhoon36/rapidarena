using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GunType { AssaultRifle = 0, Handgun, CombatKnife, Grenade }
public enum ThrowType { Default, Smoke }

[System.Serializable]
public class WeaponSetting
{
    public GunType name; // ���� �̸�
    public ThrowType Throwname; // ����ź �̸�
    public int m_Damage; // ���� ������
    public float m_AttackRate;
    public float m_AttackDist;
    public bool IsAutoAttack;
    public int m_ClipSize;
    public int m_MaxAmmo;
}

// ��� ��Ʈ�ѷ��� �θ� Ŭ����
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

    #region �÷��̾� ���¿� ���� �ִϸ��̼�
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

        // �ִϸ��̼� ���¸� ��Ʈ��ũ�� ����ȭ
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
        if (this is Player_Ctrl playerCtrl) // �÷��̾� ��Ʈ�ѷ��� ���
        {
            string anim = playerCtrl.Aim ? "AimFire" : "Fire";
            playerCtrl.SetAnimation(anim);
        }
    }

    public virtual void Reload()
    {
        // ���� ó��
        if (m_PlayerState == DefState.Reload) return;
        if (m_PlayerState == DefState.Inspect) return;
        if (m_PlayerState == DefState.Jump) return;
        if (m_PlayerState == DefState.Run) return;
        if (m_PlayerState == DefState.Walk) return;

        // ���ε� ���·� ��ȯ
        PlayerState = DefState.Reload;
        SetTrigger("Reload"); // �ִϸ��̼� Ʈ���� ����
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
    #endregion -- �⺻ ����ó��


    #region �÷��̾��� źâ ȸ��
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
