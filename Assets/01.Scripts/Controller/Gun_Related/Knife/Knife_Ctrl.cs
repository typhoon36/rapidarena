using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Knife_Ctrl : Weapon_Base
{
    Base_Ctrl m_Base;
    [SerializeField] private KnifeCollider knifeCollider;
    [SerializeField] private GameObject m_Effect;
    [SerializeField] private Transform m_KnifeTr;

    private AudioSource m_AudioSource;
    public new bool IsAttack { get; set; } // new Ű���� �߰�

    protected override void Awake() // Init
    {
        base.Awake();
        base.Setup();
        m_Base = GetComponentInParent<Player_Ctrl>();
        m_AudioSource = GetComponent<AudioSource>();
        knifeCollider.knifeCtrl = this; // KnifeCollider�� ���� ����
    }

    void OnEnable()
    {
        IsAttack = false;
        Game_Mgr.Inst.UpdateAmmoText(m_AmmoInClip, m_CurrentAmmo);

        // ���� ��ü �� �ִϸ����� �ٽ� ����
        if (m_Base != null)
            m_Base.m_Anim = GetComponent<Animator>();

        Camera.main.fieldOfView = 60;
    }

    #region Knife Action
    public override void StartWAtt(int type = 0)
    {
        if (IsAttack == true) return;

        StartCoroutine("OnAttack", type);
    }

    public override void StopWeaponAction(int type = 0)
    {
        IsAttack = false;
        StopCoroutine("OnAttack");
    }

    IEnumerator OnAttack(int type)
    {
        IsAttack = true;

        m_Base.m_Anim.SetTrigger("Attack");

        yield return new WaitForEndOfFrame();

        StartKnifeCollider(); // ���� �� �ݶ��̴� Ȱ��ȭ

        while (true)
        {
            if (m_Base.m_Anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
            {
                IsAttack = false;
                yield break;
            }
            yield return null;
        }
    }

    public override void OnAttack()
    {
        // OnAttack �޼��� ����
    }
    #endregion

    #region Reload & Inspect
    public override void Inspect()
    {
        //m_Base.Inspect();
    }

    public override void Reload()
    {
        // ź ��õ �ʿ� ����
    }
    #endregion

    #region Mode Change
    IEnumerator OnModeChange()
    {
        yield return null;
    }
    #endregion

    public void StartKnifeCollider()
    {
        knifeCollider.StartCollider(m_WeaponSetting.m_Damage);
    }

    public void SpawnEffect()
    {
        Vector3 spawnPosition = m_KnifeTr.position + new Vector3(0, 0.5f, 0); // Y������ 0.5 ���� �ø�
        GameObject m_Eff = Instantiate(m_Effect, spawnPosition, Quaternion.identity);
        Destroy(m_Eff, 2.0f); // ����Ʈ�� ���� �ð� �Ŀ� ��������� �߰�
    }
}
