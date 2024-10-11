using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponType { Main = 0, Sub,Snifer, Melee, Throw }
// 주무기, 보조무기, 근접무기, 투척무기
public enum GrenadeType { Default, Smoke }

// 무기 베이스 클래스
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
    protected Base_Ctrl m_Anim => m_BAnim; // protected로 변경

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

    #region Init
    protected virtual void Awake()
    {
        m_ASource = GetComponent<AudioSource>();
        m_BAnim = GetComponentInParent<Base_Ctrl>();
        m_CurrentAmmo = m_MaxAmmo;
        m_AmmoInClip = m_ClipSize;
    }

    protected void Setup()
    {
        m_ASource = GetComponent<AudioSource>();
        m_BAnim = GetComponentInParent<Base_Ctrl>();
    }
    #endregion

    #region Audio
    protected void PlaySound(AudioClip clip)
    {
        m_ASource.Stop();
        m_ASource.clip = clip;
        m_ASource.Play();
    }
    #endregion
}
