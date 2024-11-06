using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum GameState
{
    Ready,
    Play,
    End
}

public class Game_Mgr : MonoBehaviour
{
    #region SingleTon
    public static Game_Mgr Inst;
    void Awake()
    {
        Inst = this;
        pv = GetComponent<PhotonView>();
    }
    #endregion

    public GameObject m_GameObj;
    public GameState m_GameState = GameState.Ready;

    PhotonView pv;

    Player_Ctrl m_RefPlayer;

    #region 장탄수 
    [Header("Ammo")]
    [SerializeField] private Text m_LoadTxt;
    [SerializeField] private Text m_MaxAmmoTxt;
    #endregion

    #region HP
    [Header("HP")]
    public GameObject m_DmgPanel;
    public Image m_HPBar;
    [HideInInspector] public float m_MaxHP = 440;
    [HideInInspector] public float m_CurHP = 440;
    #endregion

    #region Gun
    [Header("Gun")]
    public Image m_AsGunImg;
    public Image m_HandGunImg;
    public Image m_KnifeImg;
    public Image m_GrenadeImg;
    public Text GunMode;
    private Weapon_Base m_Weapon;
    #endregion

    #region  Message(Only Message)
    [Header("Message")]
    public Text m_Message;
    #endregion

    #region Timer
    public Text m_Timer;
    [HideInInspector] public float m_LimitTime = 240f;
    [HideInInspector] public float m_CurTime;
    #endregion

    #region WinLose
    [Header("WinLose")]
    public Text m_WinLoseTxt;
    [HideInInspector] public int m_RoundCnt = 0;
    #endregion

    #region End
    [Header("End")]
    public Text m_GameEndText;
    #endregion

    public Text Object_Txt;

    void Start()
    {
        m_GameEndText.gameObject.SetActive(false);
        m_GameObj.SetActive(false);
        //HP초기화
        m_CurHP = m_MaxHP;

        //타이머
        m_LimitTime = 240f;
        m_Timer.text = "04:00";

        #region 이미지 초기화
        m_AsGunImg.color = Color.red; // 기본 총
        m_HandGunImg.color = Color.red; // 보조 총
        m_KnifeImg.color = Color.red; // 나이프
        m_GrenadeImg.color = Color.red; // 수류탄
        #endregion

        Object_Txt.gameObject.SetActive(false);

        // HP 바 초기화
        m_HPBar.fillAmount = m_CurHP / m_MaxHP;

        


    }

    void Update()
    {
        //타이머
        if (m_GameState == GameState.Play)
        {
            m_LimitTime -= Time.deltaTime;
            System.TimeSpan time = System.TimeSpan.FromSeconds(m_LimitTime);
            m_Timer.text = time.ToString(@"mm\:ss");

            if (m_LimitTime <= 0)
            {
                m_LimitTime = 0;
                m_GameState = GameState.End;
            }
        }

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

    #region HP
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
        m_CurHP = currentHP;
        m_MaxHP = maxHP;
        m_HPBar.fillAmount = currentHP / maxHP;
    }
    #endregion

    #region Message
    public void ShowMessage(float msg = 50f, bool IsMsg = false)
    {
        if (IsMsg == true)
        {
            m_Message.text = "장치 감지! " + msg.ToString("N1") + "초 후 폭발합니다.";
            m_Message.gameObject.SetActive(true);
            StartCoroutine(HideMsgDelay(3f));
        }
        else
        {
            m_Message.gameObject.SetActive(false);
            m_Message.text = "";
        }

    }

    IEnumerator HideMsgDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        m_Message.gameObject.SetActive(false);
        m_Message.text = "";
    }
    #endregion

}
