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
    Inven_Mgr m_Inven;//�κ��丮 �Ŵ��� ����


    void Start()
    {
        #region Top_Panel
        // ���� ��ư text ���� ����
        m_ShopBtn.GetComponentInChildren<Text>().color = Color.gray;

        // ���� ��ư ��Ȱ��ȭ 
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
        m_PointTxt.text = "���� ����Ʈ : " 
            + Data_Mgr.Inst.userData.Points.ToString();

        //��ǰ �ε�
        SpawnProducts();

        //�κ� ���� ����
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

        RefreshPoint(); //��� ����Ʈ�� ������ �Ǿ����.



    }

    //��ǰ ����
    void SpawnProducts()
    {
        //������ ������ ��������
        AllItemData itemData = Data_Mgr.Inst.GetItemData();

        //������ �����͸� �̿��Ͽ� ��ǰ ����
        foreach (var item in itemData.Sheet1)
        {
            GameObject a_Products = Instantiate(ProductObj, productParent);
            Product_Nd productNd = a_Products.GetComponent<Product_Nd>();
            productNd.SetItemData(item);
        }
    }

    //�޽��� ǥ�� -- ����/�Ǹ������� ȣ��
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

    //����Ʈ ����
    public void RefreshPoint()
    {
        m_PointTxt.text = "���� ����Ʈ : " +
            Data_Mgr.Inst.userData.Points.ToString();
    }

}
