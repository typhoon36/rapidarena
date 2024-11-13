using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//인벤토리 슬롯
public class Slot : MonoBehaviour
{
    [HideInInspector] public int m_SlotID; // 슬롯 ID 추가
    public Image m_ItemImg; // 아이템 이미지
    public Text m_ItemCount; // 아이템 갯수
    ItemData m_ItemData;

    public bool IsSelected { get; set; } // 슬롯 선택 여부

    public bool IsEquipSlot { get; set; } //마우스 우측클릭으로 장착했는지 여부

    #region Singleton
    public static Slot Inst;
    void Awake()
    {
        Inst = this;
    }
    #endregion

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            EquipSlot();
        }
    }

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

    public void ClearSlot()
    {
        m_ItemData = null;
        m_ItemImg.sprite = null;
        m_ItemImg.gameObject.SetActive(false);
        m_ItemCount.text = "";
        m_ItemCount.gameObject.SetActive(false);
        IsSelected = false; // 슬롯 선택 해제
    }

    #region 슬롯 선택/해제
    public void SelectSlot()
    {
        IsSelected = true;
        //선택되었는지를 위한 색상 변경
        m_ItemImg.color = Color.green;
    }

    public void DeselectSlot()
    {
        IsSelected = false;
        m_ItemImg.color = Color.white;
    }
    #endregion

    // 아이템이 있는지 확인
    public bool HasItem()
    {
        return m_ItemData != null;
    }

    // 아이템 데이터 가져오기
    public ItemData GetItem()
    {
        return m_ItemData;
    }

    #region 슬롯 장착
    public void EquipSlot()
    {
        //우클릭시 아이템을 장착
        if (IsEquipSlot)
        {
            IsEquipSlot = true;

            //장착된 아이템을 EquipSlot으로 이동
            Equip_Slot.Inst.AddEItem(m_ItemData);
            ClearSlot();
        }
       
    }
    #endregion


}
