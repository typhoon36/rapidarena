using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 무기 종류
public enum WeaponType { Main = 0, Sub, Melee, Throw }
// 수류탄 종류
public enum GrenadeType { Default, Smoke }


public abstract class Weapon_Base : MonoBehaviour 
{
    [Header("Weapon Base")]
    [SerializeField] protected WeaponType m_WeaponType;
    [SerializeField] protected GrenadeType m_GrenadeType;
    [SerializeField] protected WeaponSetting m_WeaponSetting;

    protected float m_LastAttackTime = 0;
    protected bool IsReload = false;
    protected bool IsAttack = false;
    protected AudioSource m_ASource;
    protected Base_Ctrl m_BAnim; 
    protected Base_Ctrl m_Anim => m_BAnim;

    [Header("Ammo Settings")]
    [SerializeField] protected int m_MaxAmmo;
    [SerializeField] protected int m_ClipSize;
    protected int m_CurrentAmmo;
    protected int m_AmmoInClip;

    public int CurrentAmmo => m_CurrentAmmo;
    public int AmmoInClip => m_AmmoInClip;
    public int MaxAmmo => m_MaxAmmo;
    public WeaponSetting WeaponSetting => m_WeaponSetting;
    public WeaponType WeaponType => m_WeaponType;

    public abstract void StartWAtt(int type = 0);
    public abstract void StopWeaponAction(int type = 0);
    public abstract void OnAttack();
    public abstract void Reload();
    public abstract void Inspect();

    protected virtual void Awake()
    {
        m_ASource = GetComponent<AudioSource>();
        m_BAnim = GetComponentInParent<Base_Ctrl>();
        m_CurrentAmmo = m_MaxAmmo;
        m_AmmoInClip = m_ClipSize;
    }

    protected virtual void LateUpdate()
    {
        // 플레이어의 회전을 따라가도록 무기의 회전 설정
        if (m_BAnim != null)
        {
            transform.rotation = m_BAnim.transform.rotation;
        }
    }

    protected void Setup()
    {
        m_ASource = GetComponent<AudioSource>();
        m_BAnim = GetComponentInParent<Base_Ctrl>();
    }


    #region Audio
    protected void PlaySound(AudioClip clip)
    {
        m_ASource.Stop();
        m_ASource.clip = clip;
        m_ASource.Play();
    }
    #endregion

  
}

