using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


// ���� 
public class HandGun_Ctrl : Weapon_Base
{
    #region Audio 
    [Header("Audio Clip")]
    [SerializeField] private AudioClip m_TakeOutSound;
    [SerializeField] private AudioClip m_FireSound;
    [SerializeField] private AudioClip m_ReloadSound;
    #endregion

    #region Fire Effect
    [Header("Fire Effect")]
    [SerializeField] private GameObject m_FireEffect;
    #endregion

    #region Casing
    [Header("SpawnPoints & CasingPool")]
    [SerializeField] private Transform m_SpawnPoint;
    private Casing_Pool m_CasingPool;
    #endregion

    #region AssultRifle`s Bullet
    [Header("Bullet")]
    [SerializeField] private GameObject m_BulletPrefab;
    #endregion

    #region Aim
    [Header("Aim")]
    [SerializeField] private Image m_AimImg;
    bool IsModeChange = false;
    float DefFov = 60;
    float AimFov = 30;
    #endregion

    Base_Ctrl m_Base;

    protected override void Awake()//Init
    {
        base.Awake();
        base.Setup();
        m_Base = GetComponentInParent<Player_Ctrl>();
        m_CasingPool = GetComponentInChildren<Casing_Pool>();
    }

    //Init �Լ�
    void ResetVar()
    {
        IsAttack = false;
        IsModeChange = false;
        if (m_Base != null)
        {
            m_Base.Aim = false;
        }
    }

    void OnEnable()
    {
        PlaySound(m_TakeOutSound);
        m_FireEffect.SetActive(false);
        UI_Mgr.Inst.UpdateAmmoText(m_AmmoInClip, m_CurrentAmmo);
        ResetVar();

        // ���� ��ü �� �ִϸ����� �ٽ� ����
        if (m_Base != null)
        {
            m_Base.m_Anim = GetComponentInChildren<Animator>();
        }
    }

    #region Handgun Action
    public override void StartWAtt(int type = 0)
    {
        if (IsModeChange) return;

        if (type == 0)
        {
            OnAttack();
        }
        else
        {
            if (IsAttack) return;
            StartCoroutine(OnModeChange());
        }
    }

    public override void StopWeaponAction(int type = 0)
    {
        if (IsModeChange) return;
        IsAttack = false;
        StopCoroutine(OnModeChange());
    }

    public override void OnAttack()
    {
        if (m_AmmoInClip > 0 && Time.time - m_LastAttackTime > m_WeaponSetting.m_AttackRate)
        {
            if (m_Base.DefState != State.DefState.Run)
            {
                m_LastAttackTime = Time.time;
                m_Base.Fire();
                PlaySound(m_FireSound);

                m_CasingPool.SpawnCase(m_SpawnPoint.position, m_SpawnPoint.right);
                m_AmmoInClip--;
                UI_Mgr.Inst.UpdateAmmoText(m_AmmoInClip, m_CurrentAmmo);

                string Anim = m_Base.Aim ? "AimFire" : "Fire";
                if (m_Base.m_Anim != null && m_Base.m_Anim.gameObject.activeInHierarchy)
                {
                    m_Base.SetAnimation(Anim);
                }


                if (!m_Base.Aim)
                {
                    StartCoroutine(ShowFireEffect());
                }
                FireBullet();
            }
        }
    }

    void FireBullet()
    {
        if (m_BulletPrefab != null && m_SpawnPoint != null)
        {
            Instantiate(m_BulletPrefab, m_SpawnPoint.position, m_SpawnPoint.rotation);
        }
    }
    #endregion

    #region Reload & Inspect
    public override void Reload()
    {
        //���� ó��
        if (m_Base.DefState == State.DefState.Reload || m_CurrentAmmo <= 0) return;
        if (m_Base.DefState == State.DefState.Inspect) return;
        if (m_Base.DefState == State.DefState.Jump) return;
        if (m_Base.DefState == State.DefState.Run) return;
        if (m_Base.DefState == State.DefState.Walk) return;

        //���ε��ϱ��� ź�� ������Ʈ
        int ammoNeeded = m_ClipSize - m_AmmoInClip;
        if (m_CurrentAmmo >= ammoNeeded)
        {
            m_CurrentAmmo -= ammoNeeded;
            m_AmmoInClip = m_ClipSize;
        }
        else
        {
            m_AmmoInClip += m_CurrentAmmo;
            m_CurrentAmmo = 0;
        }
        UI_Mgr.Inst.UpdateAmmoText(m_AmmoInClip, m_CurrentAmmo);
        //���ε� ���� 
        PlaySound(m_ReloadSound);
        //���ε� �ִϸ��̼�
        m_Base.Reload();
    }

    public override void Inspect()
    {
        m_Base.Inspect();
    }
    #endregion

    #region Coroutines
    IEnumerator ShowFireEffect()
    {
        m_FireEffect.SetActive(true);
        yield return new WaitForSeconds(m_WeaponSetting.m_AttackRate * 0.1f);
        m_FireEffect.SetActive(false);
    }

    IEnumerator OnModeChange()
    {
        float m_Cur = 0;
        float Percent = 0;
        float m_Time = 0.35f;

        m_Base.Aim = !m_Base.Aim;
        m_AimImg.enabled = !m_AimImg.enabled;

        float m_StartFov = Camera.main.fieldOfView;
        float m_EndFov = m_Base.Aim ? AimFov : DefFov;

        IsModeChange = true;

        while (Percent < 1)
        {
            m_Cur += Time.deltaTime;
            Percent = m_Cur / m_Time;
            Camera.main.fieldOfView = Mathf.Lerp(m_StartFov, m_EndFov, Percent);
            yield return null;
        }

        IsModeChange = false;
    }
    #endregion
}