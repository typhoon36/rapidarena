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
    public new bool IsAttack { get; set; } // new 키워드 추가

    protected override void Awake() // Init
    {
        base.Awake();
        base.Setup();
        m_Base = GetComponentInParent<Player_Ctrl>();
        m_AudioSource = GetComponent<AudioSource>();
        knifeCollider.knifeCtrl = this; // KnifeCollider에 참조 설정
    }

    void OnEnable()
    {
        IsAttack = false;
        Game_Mgr.Inst.UpdateAmmoText(m_AmmoInClip, m_CurrentAmmo);

        // 무기 교체 시 애니메이터 다시 설정
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

        StartKnifeCollider(); // 공격 시 콜라이더 활성화

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
        // OnAttack 메서드 구현
    }
    #endregion

    #region Reload & Inspect
    public override void Inspect()
    {
        //m_Base.Inspect();
    }

    public override void Reload()
    {
        // 탄 충천 필요 없음
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
        Vector3 spawnPosition = m_KnifeTr.position + new Vector3(0, 0.5f, 0); // Y축으로 0.5 단위 올림
        GameObject m_Eff = Instantiate(m_Effect, spawnPosition, Quaternion.identity);
        Destroy(m_Eff, 2.0f); // 이펙트가 일정 시간 후에 사라지도록 추가
    }
}
