using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Inven_Mgr : MonoBehaviour
{
    #region Top_Panel
    [Header("Top_Panel")]
    public Button m_PlayBtn;
    public Button m_LoadOutBtn;
    public Button m_ShopBtn;
    public Button m_ExitBtn;
    public Button m_HomeBtn;

    [Header("Setting")]
    public Button m_SettingBtn;
    public Transform Canvas_Parent;
    public GameObject m_ConfigObj;
    #endregion

    private void Start()
    {
        //LoadOut버튼 text색상 변경
        m_LoadOutBtn.GetComponentInChildren<Text>().color = Color.gray;

        //LoadOut버튼 비활성화 
        m_LoadOutBtn.interactable = false;


        if (m_PlayBtn != null)
            m_PlayBtn.onClick.AddListener(() =>
            {
                SceneManager.LoadScene("Pt_LobbyScene");
            });

        if (m_ShopBtn != null)
            m_ShopBtn.onClick.AddListener(() =>
            {
                SceneManager.LoadScene("ShopScene");
            });

        if (m_SettingBtn != null)
            m_SettingBtn.onClick.AddListener(() =>
            {
                Instantiate(m_ConfigObj, Canvas_Parent);
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
