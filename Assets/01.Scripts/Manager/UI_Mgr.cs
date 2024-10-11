using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// UI_Mgr.cs
public class UI_Mgr : MonoBehaviour
{
    #region SingleTon
    public static UI_Mgr Inst;
    void Awake()
    {
        Inst = this;
    }
    #endregion

    #region ��ź�� 
    [Header("Ammo")]
    [SerializeField] private Text m_LoadTxt;
    [SerializeField] private Text m_MaxAmmoTxt;
    #endregion

    #region HP
    [Header("HP")]
    public GameObject m_DmgPanel;
    public Image m_HPBar;
    float m_MaxHP = 440;
    float m_CurHP = 440;
    #endregion

    #region Gun
    [Header("Gun")]
    public Image m_AsGunImg;
    public Image m_HandGunImg;
    public Image m_KnifeImg;
    public Image m_GrenadeImg;
    public Image m_AimImg;
    public Text GunMode;
    private Weapon_Base m_Weapon;
    #endregion

    void Start()
    {
        m_HPBar.fillAmount = m_CurHP / m_MaxHP;
        m_CurHP = m_MaxHP;
        // �� �̹��� ���� ����
        m_AsGunImg.color = Color.red; // �⺻ ��
        m_HandGunImg.color = Color.red; // ���� ��
        m_KnifeImg.color = Color.red; // ������
        m_GrenadeImg.color = Color.red; // ����ź
    }

    #region Weapon
    public void SetWeapon(Weapon_Base weapon)
    {
        m_Weapon = weapon;
        UpdateAmmoText(m_Weapon.AmmoInClip, m_Weapon.CurrentAmmo);
        UpdateGunModeText(m_Weapon.WeaponType, m_Weapon.WeaponSetting.IsAutoAttack);
    }

    public void UpdateAmmoText(int ammoInClip, int currentAmmo)
    {
        m_LoadTxt.text = ammoInClip.ToString() + "/";
        m_MaxAmmoTxt.text = currentAmmo.ToString();
    }

    public void UpdateGunModeText(WeaponType weaponType, bool isAutoAttack)
    {
        switch (weaponType)
        {
            case WeaponType.Main:
                GunMode.text = isAutoAttack ? "FullAuto" : "Single";
                break;
            case WeaponType.Sub:
                GunMode.text = "Single";
                break;
            case WeaponType.Melee:
                GunMode.text = "";
                break;
            case WeaponType.Throw:
                GunMode.text = "";
                break;
        }
    }

    #endregion

    #region Damage
    public void ShowDamagePanel()
    {
        m_DmgPanel.SetActive(true);
        StartCoroutine(HideDamagePanel(1f));
    }

    private IEnumerator HideDamagePanel(float delay)
    {
        yield return new WaitForSeconds(delay);
        m_DmgPanel.SetActive(false);
    }

    public void UpdateHPBar(float currentHP, float maxHP)
    {
        m_HPBar.fillAmount = currentHP / maxHP;
    }
    #endregion

    #region InCreaseHP
    public void IncreaseHP(int HP)
    {
        m_CurHP += HP;

        m_CurHP = m_CurHP + HP > m_MaxHP ? m_MaxHP : m_CurHP + HP;

        UpdateHPBar(m_CurHP, m_MaxHP);
    }
    #endregion

    #region Ŭ������ �Լ�
    public static bool IsPointerOverUIObject()
    {
        PointerEventData a_EDCurPos = new PointerEventData(EventSystem.current);

#if !UNITY_EDITOR && (UNITY_IPHONE || UNITY_ANDROID)
        List<RaycastResult> results = new List<RaycastResult>();
        for (int i = 0; i < Input.touchCount; ++i)
        {
            a_EDCurPos.position = Input.GetTouch(i).position;  
            results.Clear();
            EventSystem.current.RaycastAll(a_EDCurPos, results);
            if (0 < results.Count)
                return true;
        }
        return false;
#else
        a_EDCurPos.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(a_EDCurPos, results);
        return (0 < results.Count);
#endif
    }
    #endregion
}
