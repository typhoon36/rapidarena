using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Equip_Slot : MonoBehaviour
{
    [HideInInspector] public int m_ESlotID; // ���� ID �߰�
    public Image m_ItemImg; // ������ �̹���
    public Image m_SlotImg; // ���� �̹���

    #region Singleton
    public static Equip_Slot Inst;
    void Awake()
    {
        Inst = this;
    }
    #endregion

    //Rifle����
    AsGun_Ctrl m_Gun;

    void Start()
    {
        //���� ������ �����ʾ����� ������ �̹����� ��Ȱ��ȭ
        m_ItemImg.gameObject.SetActive(false);
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            //��Ŭ���� ������ �������� ����
            ClearESlot();
        }
    }

    public void ClearESlot()
    {

    }

    public void AddEItem(ItemData a_Data)
    {
        if (a_Data.ItemID == 5)
        {
            SendChangeSkin(m_Gun);
        }
    }

    //��Ų�� ������Ѵ޶�� AsGun_Ctrl���� �˸�
    public void SendChangeSkin(AsGun_Ctrl a_Gun)
    {
        a_Gun.ChangeSkin();
    }



}
