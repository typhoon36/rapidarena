using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    #region UI
    public Image m_ItemImg;
    public Text m_ItemCount;
    public Button m_SlotBtn; //��� �� ������ ���� ��ư(���� Ŭ���� ���, ������ Ŭ���� ����)
    int m_SlotNum; //���� ��ȣ
    #endregion

    ItemData m_ItemData;
    Data_Mgr m_DataMgr;
    Shop_Mgr m_ShopMgr;

    void Start()
    {
        m_ItemImg.gameObject.SetActive(false);
        m_ItemCount.gameObject.SetActive(false);




    }
}
