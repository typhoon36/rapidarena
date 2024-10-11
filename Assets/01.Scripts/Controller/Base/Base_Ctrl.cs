using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 모든 컨트롤러의 부모 클래스
public class Base_Ctrl : MonoBehaviour
{
    [HideInInspector] public Animator m_Anim;

    [SerializeField]
    protected State.DefState m_PlayerState = State.DefState.Idle;

    void Awake()
    {
        m_Anim = GetComponentInChildren<Animator>();
    }

    #region 플레이어 상태에 따른 애니메이션
    public virtual State.DefState DefState
    {
        get { return m_PlayerState; }
        set
        {
            m_PlayerState = value;
            UpdateAnimationState();
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
            case State.DefState.Idle:
                m_Anim.CrossFade("Idle", 0.2f);
                break;
            case State.DefState.Walk:
                m_Anim.CrossFade("Walk", 0.2f);
                break;
            case State.DefState.Run:
                m_Anim.CrossFade("Run", 0.2f);
                break;
            case State.DefState.Fire:
                m_Anim.CrossFade("Fire", 0.2f);
                break;
            case State.DefState.Reload:
                m_Anim.CrossFade("Reload", 0.2f);
                break;
            case State.DefState.Inspect:
                m_Anim.CrossFade("Inspect", 0.2f);
                break;
        }
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
        //예외 처리
        if (m_PlayerState == State.DefState.Reload) return;

        if (m_PlayerState == State.DefState.Fire) return;

        if (m_PlayerState == State.DefState.Inspect) return;

        if (m_PlayerState == State.DefState.Jump) return;

        if (m_PlayerState == State.DefState.Run) return;

        if(m_PlayerState == State.DefState.Walk) return;

        // 리로드
        DefState = State.DefState.Reload; // 리로드 상태로 전환
    }

    public virtual void Inspect()
    {
        DefState = State.DefState.Inspect;
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
}
