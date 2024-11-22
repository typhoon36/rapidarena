using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



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

    [HideInInspector] public PhotonView pv;

    Player_Ctrl m_RefPlayer;

    #region ��ź�� 
    [Header("Ammo")]
    [SerializeField] private Text m_LoadTxt;
    [SerializeField] private Text m_MaxAmmoTxt;
    #endregion

    #region HP
    [Header("HP")]
    public GameObject m_DmgPanel;
    public Image m_HPBar;
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


    [Header("Timer")]
    public Text m_Timer;
    [HideInInspector] public float m_LimitTime = 240f;
    [HideInInspector] public float m_CurTime;

    [Header("WinLose")]
    public Text m_WinLoseTxt;

    [Header("End")]
    public Text m_GameEndText;


    [Header("Object")]
    public Text Object_Txt;

    bool IsShown = false;

    void Start()
    {
        Sound_Mgr.Inst.m_AudioSrc.clip = null;

        m_GameEndText.gameObject.SetActive(false);
        m_GameObj.SetActive(false);

        //Ÿ�̸�
        m_LimitTime = 240f;
        m_Timer.text = "04:00";

        #region �̹��� �ʱ�ȭ
        m_AsGunImg.color = Color.red; // �⺻ ��
        m_HandGunImg.color = Color.red; // ���� ��
        m_KnifeImg.color = Color.red; // ������
        m_GrenadeImg.color = Color.red; // ����ź
        #endregion

        Object_Txt.gameObject.SetActive(false);

    

        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(StartGameTimer());
        }
    }

    void Update()
    {
        // Ÿ�̸� && ų�α�
        if (m_GameState == GameState.Play)
        {
            // ���� ��ǥ �ؽ�Ʈ ���
            if (!IsShown)
            {
                if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("MyTeam"))
                {
                    string teamKind = PhotonNetwork.LocalPlayer.CustomProperties["MyTeam"].ToString();

                    if (teamKind == "blue")
                    {
                        pv.RPC("ShowTeamObjectiveRPC", RpcTarget.All, "�׷�����Ʈ ����");
                        IsShown = true;
                    }
                    else if (teamKind == "red")
                    {
                        pv.RPC("ShowTeamObjectiveRPC", RpcTarget.All, "���׷��δ� ����");
                        IsShown = true;
                    }
                }
            }
        }

        if (m_GameState == GameState.End)
        {
            StopCoroutine(StartGameTimer());
            StopCoroutine(Typing(""));
        }
    }

    [PunRPC]
    void ShowTeamObjectiveRPC(string objectiveText)
    {
        ShowTeamObjectiveText(objectiveText);
    }

    void ShowTeamObjectiveText(string objectiveText)
    {

        // �ؽ�Ʈ UI ��� Ȱ��ȭ
        Game_Mgr.Inst.Object_Txt.gameObject.SetActive(true);

        // �ڷ�ƾ ����
        StartCoroutine(Typing(objectiveText));
    }

    [PunRPC]
    void UpdateTimer(float time)
    {
        m_LimitTime = time;
        System.TimeSpan timeSpan = System.TimeSpan.FromSeconds(m_LimitTime);
        m_Timer.text = timeSpan.ToString(@"mm\:ss");
    }

    [PunRPC]
    IEnumerator StartGameTimer()
    {
        while (m_LimitTime > 0)
        {
            yield return new WaitForSeconds(1f);
            m_LimitTime -= 1f;
            pv.RPC("UpdateTimer", RpcTarget.All, m_LimitTime);
        }

        m_GameState = GameState.End;
        pv.RPC("UpdateGameState", RpcTarget.All, GameState.End);
    }

    [PunRPC]
    void UpdateGameState(GameState state)
    {
        m_GameState = state;
    }

    #region Weapon
    public void SetWeapon(Weapon_Base weapon)
    {
        m_Weapon = weapon;
        UpdateAmmoText(m_Weapon.AmmoInClip, m_Weapon.CurrentAmmo, pv);
        UpdateGunModeText(m_Weapon.WeaponType, m_Weapon.WeaponSetting.IsAutoAttack);
    }

    public void UpdateAmmoText(int ammoInClip, int currentAmmo, PhotonView pv)
    {
        if (pv.IsMine)
        {
            m_LoadTxt.text = ammoInClip.ToString() + "/ ";
            m_MaxAmmoTxt.text = currentAmmo.ToString();
        }
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
    #endregion


    IEnumerator Typing(string ObjectTxt)
    {
        // ���� ���°� �������� Ȯ��
        if (m_GameState == GameState.End)
        {
            yield break; // ������ ����Ǿ����� �ڷ�ƾ ����
        }

        yield return new WaitForSeconds(1f);

        for (int i = 0; i <= ObjectTxt.Length; i++)
        {
            // ���� ���°� �������� Ȯ��
            if (m_GameState == GameState.End)
            {
                yield break; // ������ ����Ǿ����� �ڷ�ƾ ����
            }

            Game_Mgr.Inst.Object_Txt.text = ObjectTxt.Substring(0, i);
            yield return new WaitForSeconds(0.1f);
        }

        // Typing ȿ�� ������ 3�� �Ŀ� �ؽ�Ʈ �����
        StartCoroutine(WaitText(3.0f));
    }

    IEnumerator WaitText(float delay = 10)
    {
        // ���� ���°� �������� Ȯ��
        if (m_GameState == GameState.End)
        {
            yield break; // ������ ����Ǿ����� �ڷ�ƾ ����
        }

        yield return new WaitForSeconds(delay);

        string currentText = Game_Mgr.Inst.Object_Txt.text;
        for (int i = currentText.Length; i >= 0; i--)
        {
            // ���� ���°� �������� Ȯ��
            if (m_GameState == GameState.End)
            {
                yield break; // ������ ����Ǿ����� �ڷ�ƾ ����
            }

            Game_Mgr.Inst.Object_Txt.text = currentText.Substring(0, i);
            yield return new WaitForSeconds(0.1f);
        }

        Game_Mgr.Inst.Object_Txt.gameObject.SetActive(false);
    }
}
