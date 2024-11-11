using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Inven_Mgr : MonoBehaviour
{
    #region Top_Panel(공통)
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

    [Header("Inventory")]
    public GameObject SlotObj;
    public Transform InvenParent;
    public Button AllDel_Btn;
    public Dropdown m_SortDropdown;


    void Start()
    {
        #region Top_Panel Init
        m_LoadOutBtn.GetComponentInChildren<Text>().color = Color.gray;
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
        #endregion

        #region Inventory Init

        //인벤토리 슬롯 생성
        for (int i = 0; i < 20; i++)
        {
            GameObject slot = Instantiate(SlotObj, InvenParent.transform);
            slot.name = "Slot_" + i;
        }

        //버튼 및 드롭다운 이벤트 등록
        AllDel_Btn.onClick.AddListener(() =>
        {
            Slot.Inst.ClearSlot();
        });
        m_SortDropdown.onValueChanged.AddListener((int Idx) =>
        {
            Slot.Inst.SortSlot(Idx);//추가된순 정렬
        });
        #endregion
    }

    //인벤토리 슬롯에 아이템 로드
    public void LoadInventSlots()
    {
        Slot[] m_Slots = InvenParent.GetComponents<Slot>();
        List<ItemData> m_Items = Data_Mgr.Inst.m_Items;

        for (int i = 0; i < m_Items.Count; i++)
        {
            m_Slots[i].AddItem(m_Items[i]);
            m_Slots[i].m_ItemImg.gameObject.SetActive(true);
            m_Slots[i].m_ItemCount.gameObject.SetActive(true);
        }

    }

}
