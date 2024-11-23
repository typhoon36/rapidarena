using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//�κ��丮 �Ŵ���(�κ��丮 ������ ���)
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

    [Header("Inventory")]
    public GameObject SlotObj;
    public Transform InvenParent;

    [Header("OtherUIs")]
    public Button AllDel_Btn;
    public Button AddSlot_Btn;

    [Header("Equip")]
    public GameObject EquipSlotObj;
    public Transform EquipParent;

    #region Singleton
    public static Inven_Mgr Inst;
    void Awake()
    {

        Inst = this;
    }
    #endregion

    void Start()
    {
        Data_Mgr.LoadData();

        Sound_Mgr.Inst.PlayBGM("InventoryBgm", 1f);

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

        // �κ��丮 ���� ����
        for (int i = 0; i < 20; i++)
        {
            GameObject a_SlotObj = Instantiate(SlotObj, InvenParent);
            a_SlotObj.name = "Slot_" + i;
            Slot a_Slot = a_SlotObj.GetComponent<Slot>();
            a_Slot.m_SlotID = i; // ���� ID ����

            // �κ��丮 ���Կ� �����ߴ� ������ �߰�
            if (Data_Mgr.InvenSlots.Count > i)
            {
                ItemData a_ItemData = Data_Mgr.ItemOrder.Find(x => x.ItemID == Data_Mgr.InvenSlots[i].ItemID);
                if (a_ItemData != null)
                {
                    a_Slot.AddItem(a_ItemData);
                    Debug.Log($"���� {i}�� ������ �߰���: {a_ItemData.ItemID}");
                }
            }
        }

        for (int i = 0; i < 3; i++)
        {
            GameObject a_SlotObj = Instantiate(EquipSlotObj, EquipParent);
            a_SlotObj.name = "EquipSlot_" + i;
            Equip_Slot a_Slot = a_SlotObj.GetComponent<Equip_Slot>();
            a_Slot.m_ESlotID = i; // ���� ID ����
        }

        // 1. ��ü ���� ��ư
        AllDel_Btn.onClick.AddListener(() =>
        {
            Slot[] a_Slots = InvenParent.GetComponentsInChildren<Slot>();
            for (int i = 0; i < a_Slots.Length; i++)
            {
                a_Slots[i].ClearSlot();
            }
        });

        // 2. ���� �߰� ��ư
        AddSlot_Btn.onClick.AddListener(() =>
        {
            // �߰� ��ư�� ������ �����Ǿ��ִ� ������ Ȯ�� �� �̾� ����
            AddSlot();
        });
    }

    // ���� �߰�
    public void AddSlot()
    {
        GameObject a_Slot = Instantiate(SlotObj, InvenParent);//���� �̾� ����
        a_Slot.name = "Slot_" + InvenParent.childCount;//���� �̸� ����
        Slot a_Slotsc = a_Slot.GetComponent<Slot>();//���� ������Ʈ ��������
        a_Slotsc.m_SlotID = InvenParent.childCount - 1;//���� ID ����
    }

    
}
