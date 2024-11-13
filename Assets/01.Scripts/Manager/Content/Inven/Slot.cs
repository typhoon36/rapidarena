using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//�κ��丮 ����
public class Slot : MonoBehaviour
{
    [HideInInspector] public int m_SlotID; // ���� ID �߰�
    public Image m_ItemImg; // ������ �̹���
    public Text m_ItemCount; // ������ ����
    ItemData m_ItemData;

    public bool IsSelected { get; set; } // ���� ���� ����

    public bool IsEquipSlot { get; set; } //���콺 ����Ŭ������ �����ߴ��� ����

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
            m_ItemCount.text = "1"; // �⺻������ 1���� ����
        }
        else
        {
            Debug.LogError("�̹����� �ε��� �� �����ϴ�: " + m_ItemData.ImagePath);
        }
    }

    public void ClearSlot()
    {
        m_ItemData = null;
        m_ItemImg.sprite = null;
        m_ItemImg.gameObject.SetActive(false);
        m_ItemCount.text = "";
        m_ItemCount.gameObject.SetActive(false);
        IsSelected = false; // ���� ���� ����
    }

    #region ���� ����/����
    public void SelectSlot()
    {
        IsSelected = true;
        //���õǾ������� ���� ���� ����
        m_ItemImg.color = Color.green;
    }

    public void DeselectSlot()
    {
        IsSelected = false;
        m_ItemImg.color = Color.white;
    }
    #endregion

    // �������� �ִ��� Ȯ��
    public bool HasItem()
    {
        return m_ItemData != null;
    }

    // ������ ������ ��������
    public ItemData GetItem()
    {
        return m_ItemData;
    }

    #region ���� ����
    public void EquipSlot()
    {
        //��Ŭ���� �������� ����
        if (IsEquipSlot)
        {
            IsEquipSlot = true;

            //������ �������� EquipSlot���� �̵�
            Equip_Slot.Inst.AddEItem(m_ItemData);
            ClearSlot();
        }
       
    }
    #endregion


}
