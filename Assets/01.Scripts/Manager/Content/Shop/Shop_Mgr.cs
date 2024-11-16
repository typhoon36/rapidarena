using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Shop_Mgr : MonoBehaviour
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

    #region Shop
    [Header("Message")]
    public Text m_MsgTxt;
    float ShowMsgTime;

    [Header("Products")]
    public GameObject ProductObj;
    public Transform productParent;
    #endregion

    [Header("Inventory")]
    public GameObject SlotObj;
    public Transform InvenParent;
    public Button m_SellBtn;
    public Text m_PointTxt;

    #region Ref
    Product_Nd m_Products;
    ItemData m_ItemData;
    Slot m_Slot;
    #endregion

    [HideInInspector] public int m_SvPoint;

    #region Singleton
    public static Shop_Mgr Inst;
    void Awake()
    {

        Inst = this;
        DontDestroyOnLoad(gameObject);
    }
    #endregion


    void Start()
    {
        Data_Mgr.LoadData();

        Sound_Mgr.Inst.PlayBGM("ShopBgm", 1f);

        #region Top_Panel
        m_ShopBtn.GetComponentInChildren<Text>().color = Color.gray;
        m_ShopBtn.interactable = false;

        if (m_PlayBtn != null)
        {
            m_PlayBtn.onClick.RemoveAllListeners();
            m_PlayBtn.onClick.AddListener(() =>
            {
                SceneManager.LoadScene("Pt_LobbyScene");
            });
        }

        if (m_LoadOutBtn != null)
        {
            m_LoadOutBtn.onClick.RemoveAllListeners();
            m_LoadOutBtn.onClick.AddListener(() =>
            {
                SceneManager.LoadScene("Inven_Scene");
            });
        }

        if (m_SettingBtn != null)
        {
            m_SettingBtn.onClick.RemoveAllListeners();
            m_SettingBtn.onClick.AddListener(() =>
            {
                Instantiate(m_ConfigObj, Canvas_Parent);
            });
        }

        if (m_ExitBtn != null)
        {
            m_ExitBtn.onClick.RemoveAllListeners();
            m_ExitBtn.onClick.AddListener(() =>
            {
                Application.Quit();
            });
        }

        if (m_HomeBtn != null)
        {
            m_HomeBtn.onClick.RemoveAllListeners();
            m_HomeBtn.onClick.AddListener(() =>
            {
                SceneManager.LoadScene("Pt_LobbyScene");
            });
        }
        #endregion

        if (m_PointTxt != null)
            m_PointTxt.text = "보유 포인트 :" + Data_Mgr.m_UserData.Points.ToString();

        List<ItemData> a_Data = Data_Mgr.GetItemData();

        foreach (var Data in a_Data)
        {
            GameObject a_Products = Instantiate(ProductObj, productParent);
            Product_Nd productNd = a_Products.GetComponent<Product_Nd>();
            productNd.InitData(Data);
        }

        for (int i = 0; i < 20; i++)
        {
            GameObject a_Slot = Instantiate(SlotObj, InvenParent);
            a_Slot.name = "Slot_" + i;
            Slot slotComponent = a_Slot.GetComponent<Slot>();
            slotComponent.m_SlotID = i;

            if (Data_Mgr.InvenSlots.Count > i)
            {
                ItemData a_ItemData = Data_Mgr.ItemOrder.Find(x => x.ItemID == Data_Mgr.InvenSlots[i].ItemID);
                if (a_ItemData != null)
                    slotComponent.AddItem(a_ItemData);
            }
        }

        RefreshItemList();
        RefreshInvenSlots();

        m_SellBtn.onClick.RemoveAllListeners();
        m_SellBtn.onClick.AddListener(() =>
        {
            AllSellItem();
        });
    }

    void Update()
    {
        if (ShowMsgTime > 0)
        {
            ShowMsgTime -= Time.deltaTime;
            if (ShowMsgTime <= 0 && m_MsgTxt != null)
                m_MsgTxt.text = "";
        }

        if (Slot.Inst != null)
        {
            Refresh_Slot(Slot.Inst);
        }
    }

    #region Item Buy / Sell
    public void BuyItem(ItemData a_Data)
    {
        if (Data_Mgr.m_UserData.Points < a_Data.ItemPrice)
        {
            ShowMsg("포인트가 부족합니다.");
        }
        else
        {
            Data_Mgr.m_UserData.Points -= a_Data.ItemPrice;
            m_SvPoint = Data_Mgr.m_UserData.Points;
            ShowMsg("구매 완료.");

            InvenSlot newSlot = new InvenSlot
            {
                SlotID = Data_Mgr.InvenSlots.Count,
                ItemID = a_Data.ItemID,
                ItemCount = 1,
                ImagePath = a_Data.ImagePath
            };
            Data_Mgr.InvenSlots.Add(newSlot);

            RefreshItemList();
            RefreshInvenSlots();

            if (m_PointTxt != null)
                m_PointTxt.text = "보유 포인트 :" + Data_Mgr.m_UserData.Points.ToString();

            Data_Mgr.SaveData();
        }
    }

    public void AllSellItem()
    {
        Slot[] slots = InvenParent.GetComponentsInChildren<Slot>();
        foreach (var slot in slots)
        {
            if (slot.HasItem())
            {
                ItemData itemData = slot.m_ItemData;
                Data_Mgr.m_UserData.Points += itemData.ItemPrice;
                slot.ClearSlot();
            }
        }

        Data_Mgr.InvenSlots.Clear();

        m_SvPoint = Data_Mgr.m_UserData.Points;
        if (m_PointTxt != null)
            m_PointTxt.text = "보유 포인트 :" + Data_Mgr.m_UserData.Points.ToString();
        ShowMsg("모든 아이템 판매 완료.");
        Data_Mgr.SaveData();
    }
    #endregion

    #region Refresh
    public void Refresh_Slot(Slot a_Slot)
    {
        if (Data_Mgr.InvenSlots.Count > a_Slot.m_SlotID)
        {
            ItemData a_ItemData = Data_Mgr.ItemOrder.Find(x => x.ItemID == Data_Mgr.InvenSlots[a_Slot.m_SlotID].ItemID);
            if (a_ItemData != null)
            {
                a_Slot.AddItem(a_ItemData);
            }
        }
    }

    public void RefreshInvenSlots()
    {
        Slot[] slots = InvenParent.GetComponentsInChildren<Slot>();
        foreach (var slot in slots)
        {
            Refresh_Slot(slot);
        }
    }

    void RefreshItemList()
    {
        if (productParent != null)
        {
            Product_Nd[] a_Products = productParent.GetComponentsInChildren<Product_Nd>();

            foreach (var Proudct in a_Products)
                Proudct.RefreshState();
        }
    }
    #endregion

    #region 메시지 표시
    public void ShowMsg(string msg = "", bool IsShow = true)
    {
        if (IsShow == true && m_MsgTxt != null)
        {
            m_MsgTxt.text = msg;
            m_MsgTxt.gameObject.SetActive(true);
            ShowMsgTime = 2.0f;
        }
        else if (m_MsgTxt != null)
        {
            m_MsgTxt.text = "";
            m_MsgTxt.gameObject.SetActive(false);
        }
    }
    #endregion
}
