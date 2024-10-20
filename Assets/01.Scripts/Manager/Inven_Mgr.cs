using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Inven_Mgr : MonoBehaviour
{
    #region Top_Panel
    [Header("Top_Panel")]
    public GameObject m_RoomPanel;
    public Button m_PlayBtn;
    public Button m_LoadOutBtn;
    public Button m_ShopBtn;
    public Button m_SettingBtn;
    public Button m_ExitBtn;
    public Button m_HomeBtn;
    #endregion

    #region Inven
    #endregion

    void Start()
    {
        #region Top_Panel Buttons
        if (m_PlayBtn != null)
            m_PlayBtn.onClick.AddListener(() => { SceneManager.LoadScene("Pt_LobbyScene"); });

        if (m_LoadOutBtn != null)
            m_LoadOutBtn.onClick.AddListener(() => { SceneManager.LoadScene("Inventory_Scene"); });

        if (m_ShopBtn != null)
            m_ShopBtn.onClick.AddListener(() => { SceneManager.LoadScene("ShopScene"); });

        if (m_SettingBtn != null)
            m_SettingBtn.onClick.AddListener(() => { });

        if (m_ExitBtn != null)
            m_ExitBtn.onClick.AddListener(() => { Application.Quit(); });

        if (m_HomeBtn != null)
            m_HomeBtn.onClick.AddListener(() => { SceneManager.LoadScene("Pt_LobbyScene"); });
        #endregion
    }


}
