using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Grenade_Ctrl : Weapon_Base
{
    [Header("Audio Clips")]
    [SerializeField] private AudioClip m_ThrowSound;

    [Header("Grenade")]
    [SerializeField] private GameObject[] m_GrenadePrefabs;

    [SerializeField] private Transform m_GrenadeSpawnPoint;

    Base_Ctrl m_Base;

    private void OnEnable()
    {
        IsAttack = false;
        UI_Mgr.Inst.UpdateAmmoText(m_AmmoInClip, m_CurrentAmmo);

        // ���� ��ü �� �ִϸ����� �ٽ� ����
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

    public override void StartWAtt(int type = 0)
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

        //���� �ִϸ��̼� ����
        m_Anim.SetTrigger("Attack");

        //���� ���� ���
        PlaySound(m_ThrowSound);

        yield return new WaitForSeconds(0.5f); // �ִϸ��̼��� ���� ������ ���

        // ��ź ����
        SpawnGProjectile();

        IsAttack = false;
    }

    public void SpawnGProjectile()
    {
        if (m_AmmoInClip > 0)
        {
            // ù ��° ��ź �������� ����Ͽ� �ν��Ͻ�ȭ
            GameObject a_Grenade = Instantiate(m_GrenadePrefabs[0], m_GrenadeSpawnPoint.position, m_GrenadeSpawnPoint.rotation);
            a_Grenade.GetComponent<Grenade_Projectile>().SetUp(WeaponSetting.m_Damage, transform.parent.forward);

            m_AmmoInClip = Mathf.Max(0, m_AmmoInClip - 1); // ���� ����

            UI_Mgr.Inst.UpdateAmmoText(m_AmmoInClip, m_CurrentAmmo);
        }
    }   

    public override void OnAttack() { }

    public override void Reload() { }

    public override void Inspect() { }
}