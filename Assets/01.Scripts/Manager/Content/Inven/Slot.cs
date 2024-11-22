using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

// �κ��丮 ����
public class Slot : MonoBehaviour, IPointerClickHandler
{
    [HideInInspector] public int m_SlotID; // ���� ID �߰�
    public Image m_ItemImg; // ������ �̹���
    public Text m_ItemCount; // ������ ����
    [HideInInspector]public ItemData m_ItemData;

    public bool IsEquipSlot = false; // ���콺 ����Ŭ������ �����ߴ��� ����

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
            m_ItemCount.text = "1"; // �⺻������ 1���� ����
        }
        else
        {
            Debug.LogError("�̹����� �ε��� �� �����ϴ�: " + m_ItemData.ImagePath);
        }
    }

    //���� �ʱ�ȭ
    public void ClearSlot()
    {
        m_ItemImg.sprite = null;
        m_ItemImg.gameObject.SetActive(false);
        m_ItemCount.text = "";
        m_ItemCount.gameObject.SetActive(false);
        m_ItemData = null; // ������ ������ �ʱ�ȭ
    }

    // �������� �ִ��� Ȯ��
    public bool HasItem()
    {
        return m_ItemData != null;
    }

    // ���콺 Ŭ�� �̺�Ʈ
    public void OnPointerClick(PointerEventData eventData)
    {
        // ���콺 ���� Ŭ��
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            // ���� ���Կ� �������� ���� ���
            if (IsEquipSlot)
            {
                Equip_Slot.Inst.ClearSlot();
                if (m_ItemData != null)
                {
                    Equip_Slot.Inst.AddItem(m_ItemData, this); // ���� ������ ����
                    ClearSlot();
                }
                else
                {
                    Debug.LogError("������ �����Ͱ� null�Դϴ�.");
                }
            }
        }
        // ���콺 ���� Ŭ��
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (HasItem())
            {
                List<Equip_Slot> equipSlots = new List<Equip_Slot>(FindObjectsOfType<Equip_Slot>());
                Equip_Slot emptySlot = Equip_Slot.FindEmptySlot(equipSlots);
                if (emptySlot != null)
                {
                    emptySlot.AddItem(m_ItemData, this); // ���� ������ ����
                    IsEquipSlot = true;
                    ClearSlot();
                }
                else
                {
                    Debug.Log("��� ���� ������ ���� á���ϴ�.");
                }
            }
        }
    }
}
