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
    public Text m_PointTxt;//info

    #region Ref
    Product_Nd m_Products;
    ItemData m_ItemData;
    Slot m_Slot;
    #endregion

    //포인트
    int m_SvPoint;

    #region Singleton
    public static Shop_Mgr Inst;
    void Awake()
    {
        Inst = this;
    }
    #endregion

    void Start()
    {
        Data_Mgr.LoadData();

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

        if (m_PointTxt != null)
            m_PointTxt.text = "보유 포인트 :" + Data_Mgr.m_UserData.Points.ToString();

        //아이템 데이터 가져오기
        List<ItemData> a_Data = Data_Mgr.GetItemData();

        //아이템 데이터를 이용하여 상품 생성
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
            slotComponent.m_SlotID = i; // 슬롯 ID 설정

            // 슬롯 클릭 이벤트 추가
            Button slotButton = a_Slot.GetComponent<Button>();
            if (slotButton != null)
            {
                slotButton.onClick.AddListener(() => OnSlotClick(slotComponent));
            }

            // 인벤토리 슬롯에 구매했던 아이템 추가
            if (Data_Mgr.InvenSlots.Count > i)
            {
                ItemData a_ItemData = Data_Mgr.ItemOrder.Find(x => x.ItemID == Data_Mgr.InvenSlots[i].ItemID);
                if (a_ItemData != null)
                {
                    slotComponent.AddItem(a_ItemData);
                }
            }
        }

        //동기화
        RefreshItemList();
        RefreshInvenSlots(); // 슬롯 동기화

        //판매 버튼
        m_SellBtn.onClick.AddListener(() =>
        {
            if (m_Slot != null)
            {
                // 선택된 슬롯만 판매
                SellSlot(m_Slot);
                m_Slot = null; // 선택 초기화
            }
            else
            {
                // 전체 슬롯 판매
                Slot[] a_Slots = InvenParent.GetComponentsInChildren<Slot>();
                foreach (var slot in a_Slots)
                {
                    SellSlot(slot);
                }
                ShowMsg("전체 판매 완료.");
            }

            // 로컬에 저장
            Data_Mgr.SaveData();

            RefreshInvenSlots();
        });
    }

    void Update()
    {
        //메시지 표시 타이머
        if (ShowMsgTime > 0)
        {
            ShowMsgTime -= Time.deltaTime;
            if (ShowMsgTime <= 0)
                m_MsgTxt.text = "";
        }

        Refresh_Slot(Slot.Inst);
    }


    #region Item Buy / Sell
    public void BuyItem(ItemData a_ItemData)
    {
        if (Data_Mgr.m_UserData.Points < a_ItemData.ItemPrice)
        {
            ShowMsg("포인트가 부족합니다.");
        }
        else
        {
            Data_Mgr.m_UserData.Points -= a_ItemData.ItemPrice;
            m_SvPoint = Data_Mgr.m_UserData.Points;
            ShowMsg("구매 완료.");

            // 인벤토리 슬롯에 아이템 추가
            InvenSlot newSlot = new InvenSlot
            {
                SlotID = Data_Mgr.InvenSlots.Count,
                ItemID = a_ItemData.ItemID,
                ItemCount = 1,
                ImagePath = a_ItemData.ImagePath
            };
            Data_Mgr.InvenSlots.Add(newSlot);

            RefreshItemList();
            RefreshInvenSlots(); // 슬롯 동기화

            m_PointTxt.text = "보유 포인트 :" + Data_Mgr.m_UserData.Points.ToString();

            // 로컬에 저장
            Data_Mgr.SaveData();
        }
    }


    void SellSlot(Slot a_Slot)
    {
        if (a_Slot.HasItem())
        {
            UserData userData = Data_Mgr.m_UserData;
            userData.Points += a_Slot.GetItem().ItemPrice;
            a_Slot.ClearSlot();
            Data_Mgr.InvenSlots.RemoveAll(x => x.SlotID == a_Slot.m_SlotID);

            m_PointTxt.text = "보유 포인트 :" + Data_Mgr.m_UserData.Points.ToString();

            // 로컬에 저장
            Data_Mgr.SaveData();

            // 슬롯 선택 해제
            a_Slot.DeselectSlot();
        }
    }
    #endregion

    #region Refresh
    public void Refresh_Slot(Slot a_Slot)
    {
        // 인벤토리 슬롯에 구매했던 아이템 추가
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


    #region 메시지 표시 -- 구매했을때 호출
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
    #endregion

    // 슬롯 클릭 이벤트
    void OnSlotClick(Slot slot)
    {
        if (m_Slot != null)
        {
            m_Slot.DeselectSlot();
        }

        m_Slot = slot;
        m_Slot.SelectSlot();
    }



}
