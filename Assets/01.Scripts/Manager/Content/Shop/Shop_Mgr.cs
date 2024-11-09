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
    [Header("Setting")]
    public Button m_SettingBtn;
    public Transform Canvas_Parent;
    public GameObject m_ConfigObj;
    #endregion

    public Text m_MsgTxt;
    float ShowMsgTimer = 0.0f;

    #region Products
    [Header("Products")]
    public GameObject ProductObj;
    public Transform productParent;
    #endregion

    #region Inventory
    Inven_Mgr m_Inven;//�κ��丮�� ����(�κ��丮�� �ִ� �������� Inven_View�� ǥ��)
    public Transform InvenParent;//�κ��丮 �θ�
    public GameObject SlotObj;//���� ������Ʈ 
    public Text m_PointTxt;//����Ʈ ǥ��
    #endregion

    void Start()
    {
        #region Top_Panel
        //���� ��ư text���� ����
        m_ShopBtn.GetComponentInChildren<Text>().color = Color.gray;

        //���� ��ư ��Ȱ��ȭ 
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

        // ����Ʈ�� 99999�� ����
        Data_Mgr.Inst.userData.Points = 99999;
        // ����Ʈ ǥ��
        m_PointTxt.text = Data_Mgr.Inst.userData.Points.ToString();

        // ������ ����
        SpawnProducts();
    }

    void Update()
    {
        if (ShowMsgTimer > 0.0f)
        {
            ShowMsgTimer -= Time.deltaTime;
            if (ShowMsgTimer <= 0.0f)
            {
                m_MsgTxt.text = "";
            }
        }
    }

    public void ShowMsg(string a_Msg = "", bool IsTrigger = true)
    {
        if (IsTrigger == true)
        {
            m_MsgTxt.text = a_Msg;
            m_MsgTxt.gameObject.SetActive(true);
            ShowMsgTimer = 2.0f;
        }
        else
        {
            m_MsgTxt.text = "";
            m_MsgTxt.gameObject.SetActive(false);
        }
    }

    void SpawnProducts()
    {
        AllItemData itemData = Data_Mgr.Inst.GetItemData();

        foreach (var item in itemData.Sheet1)
        {
            GameObject product = Instantiate(ProductObj, productParent);
            Product_Nd productNd = product.GetComponentInChildren<Product_Nd>();
            productNd.SetItemData(item);

        }
    }

    // �� ������ ã�� �޼��� �߰�
    public Slot FindEmptySlot()
    {
        Slot[] slots = InvenParent.GetComponentsInChildren<Slot>();
        foreach (Slot slot in slots)
        {
            if (!slot.m_ItemImg.gameObject.activeSelf)
            {
                return slot;
            }
        }
        return null;
    }
}
