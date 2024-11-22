using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Equip_Slot : MonoBehaviour, IPointerClickHandler
{
    [HideInInspector] public int m_ESlotID; // 장착 슬롯 ID 
    [HideInInspector] public bool IsEmpty = true; // 슬롯이 비어있는지 여부
    //UI
    public Image m_ItemImg; // 아이템 이미지
    public Image m_SlotImg; // 슬롯 이미지

    //Reference
    ItemData m_ItemData; //아이템 데이터 참조
    Slot m_OriginSlot; // 인벤 슬롯 추적

    #region Singleton
    public static Equip_Slot Inst;
    void Awake()
    {
        Inst = this;
    }
    #endregion

    void Start()
    {
        //Init
        m_ItemImg.gameObject.SetActive(false);
    }

    // 아이템 데이터 가져오기
    public ItemData GetItem()
    {
        return m_ItemData;
    }

    public void AddItem(ItemData a_Data, Slot a_OriginSlot = null)
    {
        //아이템 데이터가 없으면 리턴
        if (a_Data == null) return;

        //아이템 이미지 로드
        Sprite a_Sprite = Resources.Load<Sprite>(a_Data.ImagePath);

        if (a_Sprite != null)
        {
            m_ItemImg.sprite = a_Sprite;//아이템 이미지 설정
            m_ItemImg.gameObject.SetActive(true);//설정 후 활성화
            IsEmpty = false;//장착되었으니 비어있지 않다고 알려줌
            m_ItemData = a_Data; // 아이템 데이터 저장
            m_OriginSlot = a_OriginSlot; // 원래 슬롯 저장
        }
    }

    public void ClearSlot()
    {
        m_ItemImg.sprite = null;
        m_ItemImg.gameObject.SetActive(false);//
        IsEmpty = true;//장착해제 되었으니 비어있다고 알림
        m_ItemData = null; // 아이템 데이터 초기화
        m_OriginSlot = null; // 원래 슬롯 초기화
    }

    // 첫 번째 비어있는 장착 슬롯을 찾는 메서드
    public static Equip_Slot FindEmptySlot(List<Equip_Slot> a_ESlots)
    {
        foreach (var Slot in a_ESlots)
        {
            // 비어있는 슬롯을 찾으면 슬롯을 반환
            if (Slot.IsEmpty == true)
            {
                return Slot;
            }
        }
        //없다면 null 
        return null;
    }


    public void OnPointerClick(PointerEventData eData)
    {
        //마우스 오른쪽 클릭
        if (eData.button == PointerEventData.InputButton.Right)
        {
            //장착 슬롯이 비어있지 않다고 판단
            if (IsEmpty == false)
            {
                // 장비를 해제하면 원래 슬롯으로 이동
                if (m_OriginSlot != null)
                {
                    ItemData a_Data = GetItem(); // 현재 아이템 데이터를 저장
                    m_OriginSlot.AddItem(a_Data); // 원래 슬롯에 아이템 추가
                    ClearSlot(); // 장비 슬롯 초기화
                }
            }
        }
    }


}
