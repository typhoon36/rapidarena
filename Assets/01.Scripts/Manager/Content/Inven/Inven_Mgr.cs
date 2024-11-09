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

    #region Inventory 
    [Header("Inventory")]
    public GameObject m_Inventory;
    public GameObject m_Slot;
    #endregion

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
        for (int i = 0; i < 20; i++)
        {
            GameObject slot = Instantiate(m_Slot, m_Inventory.transform);
            slot.name = "Slot_" + i;
        }
        #endregion

        Data_Mgr.Inst.LoadInventoryItems();
        LoadInventoryItemsToSlots();
    }

    void OnEnable()
    {
        Data_Mgr.Inst.LoadInventoryItems();
        LoadInventoryItemsToSlots();
    }

    void LoadInventoryItemsToSlots()
    {
        Slot[] slots = m_Inventory.GetComponentsInChildren<Slot>();
        List<ItemData> inventoryItems = Data_Mgr.Inst.inventoryItems;

        for (int i = 0; i < inventoryItems.Count; i++)
        {
            if (i < slots.Length)
            {
                slots[i].AddItem(inventoryItems[i]);
                slots[i].m_ItemImg.gameObject.SetActive(true);
                slots[i].m_ItemCount.gameObject.SetActive(true);
            }
        }
    }
}
