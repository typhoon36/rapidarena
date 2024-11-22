using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

// 권총 
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

    #region Handgun`s Bullet
    [Header("Bullet")]
    [SerializeField] private GameObject m_BulletPrefab;
    [SerializeField] private GameObject m_BulletSpawnPoint;
    #endregion

    #region Aim
    bool IsModeChange = false;
    float DefFov = 60;
    float AimFov = 30;
    #endregion

    Base_Ctrl m_Base;
    int m_AttackerId = -1;
    [HideInInspector] public string AttackerTeam = "blue";


    protected override void Awake()//Init
    {
        base.Awake();
        base.Setup();
        m_Base = GetComponentInParent<Player_Ctrl>();
        m_CasingPool = GetComponentInChildren<Casing_Pool>();
    }

    protected override void LateUpdate()
    {
        base.LateUpdate();
    }

    //Init 함수
    void ResetVar()
    {
        IsAttack = false;
        IsModeChange = false;
        if (m_Base != null)
        {
            m_Base.Aim = false;
        }
    }

    public override void OnEnable()
    {
        base.OnEnable();

        if (photonView.IsMine)
        {
            Game_Mgr.Inst.UpdateAmmoText(m_AmmoInClip, m_CurrentAmmo, photonView);
        }


        if (m_TakeOutSound != null)
        {
            PlaySound(m_TakeOutSound);
        }

        if (m_FireEffect != null)
        {
            m_FireEffect.SetActive(false);
        }

        Game_Mgr.Inst.UpdateAmmoText(m_AmmoInClip, m_CurrentAmmo, photonView);
        ResetVar();

        if (m_Base != null)
        {
            m_Base.m_Anim = GetComponentInChildren<Animator>();
        }
    }

    #region Handgun Action
    public override void StartWAtt(int type = 0, int a_AttackerId = -1)
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
            if (m_Base.PlayerState != Base_Ctrl.DefState.Run)
            {
                m_LastAttackTime = Time.time;
                m_Base.Fire();
                PlaySound(m_FireSound);

                m_CasingPool.SpawnCase(m_SpawnPoint.position, m_SpawnPoint.right);
                m_AmmoInClip--;
                Game_Mgr.Inst.UpdateAmmoText(m_AmmoInClip, m_CurrentAmmo, photonView);

                string Anim = m_Base.Aim ? "AimFire" : "Fire";
                m_Base.SetAnimation(Anim);


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

        if (m_BulletPrefab != null && m_BulletSpawnPoint != null)
        {
            GameObject bullet = Instantiate(m_BulletPrefab, m_BulletSpawnPoint.transform.position, Quaternion.identity);
            bullet.transform.forward = m_BulletSpawnPoint.transform.forward;

            Bullet_Ctrl bulletCtrl = bullet.GetComponent<Bullet_Ctrl>();
            if (bulletCtrl != null)
            {
                bulletCtrl.AttackerId = m_AttackerId;
                if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("MyTeam"))
                {
                    bulletCtrl.AttackerTeam = (string)PhotonNetwork.LocalPlayer.CustomProperties["MyTeam"];
                }
            }
        }
    }
    #endregion

    #region Reload & Inspect
    public override void Reload()
    {
        //예외 처리
        if (m_Base.PlayerState == Base_Ctrl.DefState.Reload || m_CurrentAmmo <= 0) return;
        if (m_Base.PlayerState == Base_Ctrl.DefState.Inspect) return;
        if (m_Base.PlayerState == Base_Ctrl.DefState.Jump) return;
        if (m_Base.PlayerState == Base_Ctrl.DefState.Run) return;
        if (m_Base.PlayerState == Base_Ctrl.DefState.Walk) return;

        //리로드하기전 탄약 업데이트
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
        Game_Mgr.Inst.UpdateAmmoText(m_AmmoInClip, m_CurrentAmmo, photonView);
        //리로드 사운드 
        PlaySound(m_ReloadSound);
        //리로드 애니메이션
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
