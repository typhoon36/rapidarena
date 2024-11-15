using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
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

    #region 장탄수 
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


    [Header("Message")]
    public Text m_Message;

    [Header("Timer")]
    public Text m_Timer;
    [HideInInspector] public float m_LimitTime = 240f;
    [HideInInspector] public float m_CurTime;

    [Header("WinLose")]
    public Text m_WinLoseTxt;


    [Header("End")]
    public Text m_GameEndText;

    [Header("KillBoard")]
    public GameObject m_KillBoard;
    public Text m_Player1Kill;
    public Text m_Player2Kill;
    public Text m_Player3Kill;
    public Text m_Player4Kill;
    Dictionary<int, int> playerKillCounts = new Dictionary<int, int>();



    [Header("Object")]
    public Text Object_Txt;


    void Start()
    {
        m_GameEndText.gameObject.SetActive(false);
        m_GameObj.SetActive(false);


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


        #region KillBoard 초기화
        m_KillBoard.SetActive(false);
        m_Player1Kill.text = pv.OwnerActorNr.ToString() + " : 0";
        m_Player2Kill.text = pv.OwnerActorNr.ToString() + " : 0";
        m_Player3Kill.text = pv.OwnerActorNr.ToString() + " : 0";
        m_Player4Kill.text = pv.OwnerActorNr.ToString() + " : 0";

        playerKillCounts.Clear();
        #endregion


        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(StartGameTimer());
        }

    }

    void Update()
    {


        //타이머 && 킬로그
        if (m_GameState == GameState.Play && pv.IsMine)
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                m_KillBoard.SetActive(true);
            }
            else if (Input.GetKeyUp(KeyCode.Tab))
            {
                m_KillBoard.SetActive(false);
            }
            //팀별 목표텍스트 출력
            string a_TeamKind = "blue";

            if (Object_Txt != null && pv.IsMine)
            {
                Object_Txt.gameObject.SetActive(true);
                if (a_TeamKind == "blue")
                {
                    StartCoroutine(Typing("테러리스트 저지"));
                }
                else if (a_TeamKind == "red")
                {
                    StartCoroutine(Typing("대테러부대 저지"));
                }
            }

        }

        if (m_GameState == GameState.End)
        {
            KillRank();
        }

    }

    [PunRPC]
    void UpdateTimer(float time)
    {
        m_LimitTime = time;
        System.TimeSpan timeSpan = System.TimeSpan.FromSeconds(m_LimitTime);
        m_Timer.text = timeSpan.ToString(@"mm\:ss");
    }
    [PunRPC]
    void UpdateObjectTxt(string objectTxt)
    {
        StartCoroutine(Typing(objectTxt));
    }

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

    #region KillBoard
    public void UpdateKillCount(int playerId, int killCount)
    {
        if (playerKillCounts.ContainsKey(playerId))
        {
            playerKillCounts[playerId] = killCount;
        }
        else
        {
            playerKillCounts.Add(playerId, killCount);
        }
    }
    void KillRank()
    {
        var sortedKills = playerKillCounts.OrderByDescending(x => x.Value).ToList();

        for (int i = 0; i < sortedKills.Count; i++)
        {
            switch (i)
            {
                case 0:
                    m_Player1Kill.text = $"Player {sortedKills[i].Key} : {sortedKills[i].Value}";
                    break;
                case 1:
                    m_Player2Kill.text = $"Player {sortedKills[i].Key} : {sortedKills[i].Value}";
                    break;
                case 2:
                    m_Player3Kill.text = $"Player {sortedKills[i].Key} : {sortedKills[i].Value}";
                    break;
                case 3:
                    m_Player4Kill.text = $"Player {sortedKills[i].Key} : {sortedKills[i].Value}";
                    break;
            }
        }
    }
    #endregion

    #region 연출

    IEnumerator WaitText(float delay = 10)
    {
        string currentText = Game_Mgr.Inst.Object_Txt.text;
        for (int i = currentText.Length; i >= 0; i--)
        {
            Game_Mgr.Inst.Object_Txt.text = currentText.Substring(0, i);
            yield return new WaitForSeconds(0.1f);
        }

        Game_Mgr.Inst.Object_Txt.gameObject.SetActive(false);
    }

    IEnumerator Typing(string ObjectTxt)
    {
        yield return new WaitForSeconds(1f);

        for (int i = 0; i <= ObjectTxt.Length; i++)
        {
            Game_Mgr.Inst.Object_Txt.text = ObjectTxt.Substring(0, i);
            yield return new WaitForSeconds(0.1f);
        }

        // Typing 효과 끝나고 3초 후에 텍스트 사라짐
        StartCoroutine(WaitText(3.0f));
    }
    #endregion
}
