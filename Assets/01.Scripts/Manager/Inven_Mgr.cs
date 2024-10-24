using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Inven_Mgr : MonoBehaviour
{
    #region Top_Panel
    public Button m_PlayBtn;
    public Button m_LoadOutBtn;
    public Button m_ShopBtn;
    public Button m_SettingBtn;
    public Button m_ExitBtn;
    public Button m_HomeBtn;
    #endregion

    private void Start()
    {
        if (m_PlayBtn != null)
            m_PlayBtn.onClick.AddListener(() =>
            {
                SceneManager.LoadScene("Pt_LobbyScene");
            });

        if (m_LoadOutBtn != null)
            m_LoadOutBtn.onClick.AddListener(() =>
            {
                SceneManager.LoadScene("Inven_Scene");
            });

        if (m_ShopBtn != null)
            m_ShopBtn.onClick.AddListener(() =>
            {
                SceneManager.LoadScene("ShopScene");
            });

        if (m_SettingBtn != null)
            m_SettingBtn.onClick.AddListener(() =>
            {
                //ConfigBox ½ºÆù
            });

        if (m_ExitBtn != null)
            m_ExitBtn.onClick.AddListener(() =>
            {
                Application.Quit();
            });

        if (m_HomeBtn != null)
            m_HomeBtn.onClick.AddListener(() =>
            {
                SceneManager.LoadScene("Pt_LobbyScene");
            });
        
    }
}
