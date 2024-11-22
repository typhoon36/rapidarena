using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

// 인벤토리 슬롯
public class Slot : MonoBehaviour, IPointerClickHandler
{
    [HideInInspector] public int m_SlotID; // 슬롯 ID 추가
    public Image m_ItemImg; // 아이템 이미지
    public Text m_ItemCount; // 아이템 갯수
    [HideInInspector]public ItemData m_ItemData;

    public bool IsEquipSlot = false; // 마우스 우측클릭으로 장착했는지 여부

    #region Singleton
    public static Slot Inst;
    void Awake()
    {
        Inst = this;
    }
    #endregion

    public void AddItem(ItemData a_Data)
    {
        m_ItemData = a_Data;
        Sprite itemSprite = Resources.Load<Sprite>(m_ItemData.ImagePath);
        if (itemSprite != null)
        {
            m_ItemImg.sprite = itemSprite;
            m_ItemImg.gameObject.SetActive(true);
            m_ItemCount.gameObject.SetActive(true);
            m_ItemCount.text = "1"; // 기본적으로 1개로 설정
        }
        else
        {
            Debug.LogError("이미지를 로드할 수 없습니다: " + m_ItemData.ImagePath);
        }
    }

    //슬롯 초기화
    public void ClearSlot()
    {
        m_ItemImg.sprite = null;
        m_ItemImg.gameObject.SetActive(false);
        m_ItemCount.text = "";
        m_ItemCount.gameObject.SetActive(false);
        m_ItemData = null; // 아이템 데이터 초기화
    }

    // 아이템이 있는지 확인
    public bool HasItem()
    {
        return m_ItemData != null;
    }

    // 마우스 클릭 이벤트
    public void OnPointerClick(PointerEventData eventData)
    {
        // 마우스 왼쪽 클릭
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            // 장착 슬롯에 아이템이 있을 경우
            if (IsEquipSlot)
            {
                Equip_Slot.Inst.ClearSlot();
                if (m_ItemData != null)
                {
                    Equip_Slot.Inst.AddItem(m_ItemData, this); // 원래 슬롯을 전달
                    ClearSlot();
                }
                else
                {
                    Debug.LogError("아이템 데이터가 null입니다.");
                }
            }
        }
        // 마우스 우측 클릭
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (HasItem())
            {
                List<Equip_Slot> equipSlots = new List<Equip_Slot>(FindObjectsOfType<Equip_Slot>());
                Equip_Slot emptySlot = Equip_Slot.FindEmptySlot(equipSlots);
                if (emptySlot != null)
                {
                    emptySlot.AddItem(m_ItemData, this); // 원래 슬롯을 전달
                    IsEquipSlot = true;
                    ClearSlot();
                }
                else
                {
                    Debug.Log("모든 장착 슬롯이 가득 찼습니다.");
                }
            }
        }
    }
}
