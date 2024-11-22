using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Equip_Slot : MonoBehaviour, IPointerClickHandler
{
    [HideInInspector] public int m_ESlotID; // ���� ���� ID 
    [HideInInspector] public bool IsEmpty = true; // ������ ����ִ��� ����
    //UI
    public Image m_ItemImg; // ������ �̹���
    public Image m_SlotImg; // ���� �̹���

    //Reference
    ItemData m_ItemData; //������ ������ ����
    Slot m_OriginSlot; // �κ� ���� ����

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

    // ������ ������ ��������
    public ItemData GetItem()
    {
        return m_ItemData;
    }

    public void AddItem(ItemData a_Data, Slot a_OriginSlot = null)
    {
        //������ �����Ͱ� ������ ����
        if (a_Data == null) return;

        //������ �̹��� �ε�
        Sprite a_Sprite = Resources.Load<Sprite>(a_Data.ImagePath);

        if (a_Sprite != null)
        {
            m_ItemImg.sprite = a_Sprite;//������ �̹��� ����
            m_ItemImg.gameObject.SetActive(true);//���� �� Ȱ��ȭ
            IsEmpty = false;//�����Ǿ����� ������� �ʴٰ� �˷���
            m_ItemData = a_Data; // ������ ������ ����
            m_OriginSlot = a_OriginSlot; // ���� ���� ����
        }
    }

    public void ClearSlot()
    {
        m_ItemImg.sprite = null;
        m_ItemImg.gameObject.SetActive(false);//
        IsEmpty = true;//�������� �Ǿ����� ����ִٰ� �˸�
        m_ItemData = null; // ������ ������ �ʱ�ȭ
        m_OriginSlot = null; // ���� ���� �ʱ�ȭ
    }

    // ù ��° ����ִ� ���� ������ ã�� �޼���
    public static Equip_Slot FindEmptySlot(List<Equip_Slot> a_ESlots)
    {
        foreach (var Slot in a_ESlots)
        {
            // ����ִ� ������ ã���� ������ ��ȯ
            if (Slot.IsEmpty == true)
            {
                return Slot;
            }
        }
        //���ٸ� null 
        return null;
    }


    public void OnPointerClick(PointerEventData eData)
    {
        //���콺 ������ Ŭ��
        if (eData.button == PointerEventData.InputButton.Right)
        {
            //���� ������ ������� �ʴٰ� �Ǵ�
            if (IsEmpty == false)
            {
                // ��� �����ϸ� ���� �������� �̵�
                if (m_OriginSlot != null)
                {
                    ItemData a_Data = GetItem(); // ���� ������ �����͸� ����
                    m_OriginSlot.AddItem(a_Data); // ���� ���Կ� ������ �߰�
                    ClearSlot(); // ��� ���� �ʱ�ȭ
                }
            }
        }
    }


}
