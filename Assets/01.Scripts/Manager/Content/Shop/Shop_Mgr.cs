using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Shop_Mgr : MonoBehaviour
{
    #region Singleton
    public static Shop_Mgr Inst;
    void Awake()
    {
        Inst = this;
    }
    #endregion

    #region Top_Panel
    [Header("Top_Panel")]
    public Button m_PlayBtn;
    public Button m_LoadOutBtn;
    public Button m_ShopBtn;
    public Button m_ExitBtn;
    public Button m_HomeBtn;

    #endregion

    [Header("Setting")]
    public Button m_SettingBtn;
    public Transform Canvas_Parent;
    public GameObject m_ConfigObj;

    [Header("Message")]
    public Text m_MsgTxt;
    float ShowMsgTime;

    [Header("Products")]
    public GameObject ProductObj;
    public Transform productParent;


    [Header("Inventory")]
    public Transform InvenParent;
    public GameObject SlotObj;
    public Text m_PointTxt;//info
    Inven_Mgr m_Inven;//인벤토리 매니저 참조


    void Start()
    {
        #region Top_Panel
        // 상점 버튼 text 색상 변경
        m_ShopBtn.GetComponentInChildren<Text>().color = Color.gray;

        // 상점 버튼 비활성화 
        m_ShopBtn.interactable = false;

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
        #endregion

        //Info
        m_PointTxt.text = "보유 포인트 : " 
            + Data_Mgr.Inst.userData.Points.ToString();

        //상품 로드
        SpawnProducts();

        //인벤 슬롯 생성
        for (int i = 0; i < 20; i++)
        {
            Slot a_Slot = Instantiate(SlotObj, InvenParent).GetComponent<Slot>();
            a_Slot.m_ItemImg.gameObject.SetActive(false);
        }

    }
    void Update()
    {
        if (ShowMsgTime > 0.0f)
        {
            ShowMsgTime -= Time.deltaTime;
            if (ShowMsgTime <= 0.0f)
            {
                m_MsgTxt.text = "";
            }
        }

        RefreshPoint(); //계속 포인트가 갱신이 되어야함.



    }

    //상품 생성
    void SpawnProducts()
    {
        //아이템 데이터 가져오기
        AllItemData itemData = Data_Mgr.Inst.GetItemData();

        //아이템 데이터를 이용하여 상품 생성
        foreach (var item in itemData.Sheet1)
        {
            GameObject a_Products = Instantiate(ProductObj, productParent);
            Product_Nd productNd = a_Products.GetComponent<Product_Nd>();
            productNd.SetItemData(item);
        }
    }

    //메시지 표시 -- 구매/판매했을때 호출
    public void ShowMsg(string msg = "", bool IsShow = true)
    {
        if (IsShow == true)
        {
            m_MsgTxt.text = msg;
            m_MsgTxt.gameObject.SetActive(true);
            ShowMsgTime = 2.0f;
        }
        else
        {
            m_MsgTxt.text = "";
            m_MsgTxt.gameObject.SetActive(false);
        }

    }

    //포인트 갱신
    public void RefreshPoint()
    {
        m_PointTxt.text = "보유 포인트 : " +
            Data_Mgr.Inst.userData.Points.ToString();
    }

}
