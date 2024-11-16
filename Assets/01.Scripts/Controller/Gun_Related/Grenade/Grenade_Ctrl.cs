using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade_Ctrl : Weapon_Base
{
    [Header("Audio Clips")]
    [SerializeField] private AudioClip m_ThrowSound;

    [Header("Grenade")]
    [SerializeField] private GameObject[] m_GrenadePrefabs;

    [SerializeField] private Transform m_GrenadeSpawnPoint;

    Base_Ctrl m_Base;


    public override void OnEnable()
    {
        IsAttack = false;
        Game_Mgr.Inst.UpdateAmmoText(m_AmmoInClip, m_CurrentAmmo,photonView);

        if (photonView.IsMine)
        {
            Game_Mgr.Inst.UpdateAmmoText(m_AmmoInClip, m_CurrentAmmo, photonView);
        }

        // 무기 교체 시 애니메이터 다시 설정
        if (m_Base != null)
            m_Base.m_Anim = GetComponent<Animator>();


        Camera.main.fieldOfView = 60;

    }


    protected override void Awake()
    {
        base.Awake();
        base.Setup();
        m_Base = GetComponentInParent<Player_Ctrl>();
    }

    protected override void LateUpdate()
    {
        base.LateUpdate();
    }

    public override void StartWAtt(int type = 0, int a_AttackerId = -1)
    {
        if (type == 0 && IsAttack == false && m_AmmoInClip > 0)
        {
            StartCoroutine(AttackCo());
        }
    }

    public override void StopWeaponAction(int type = 0) { }

    private IEnumerator AttackCo()
    {
        IsAttack = true;

        //공격 애니메이션 실행
        m_Anim.SetTrigger("Attack");

        //공격 사운드 재생
        PlaySound(m_ThrowSound);

        yield return new WaitForSeconds(0.5f); // 애니메이션이 끝날 때까지 대기

        // 폭탄 스폰
        SpawnGProjectile();

        IsAttack = false;
    }

    public void SpawnGProjectile()
    {
        if (m_AmmoInClip > 0)
        {
            // 첫 번째 폭탄 프리팹을 사용하여 인스턴스화
            GameObject a_Grenade = Instantiate(m_GrenadePrefabs[0], m_GrenadeSpawnPoint.position, m_GrenadeSpawnPoint.rotation);
            a_Grenade.GetComponent<Grenade_Projectile>().SetUp(WeaponSetting.m_Damage, transform.parent.forward);

            m_AmmoInClip = Mathf.Max(0, m_AmmoInClip - 1); // 음수 방지

            Game_Mgr.Inst.UpdateAmmoText(m_AmmoInClip, m_CurrentAmmo, photonView);
        }
    }

    public override void OnAttack() { }

    public override void Reload() { }

    public override void Inspect() { }
}
