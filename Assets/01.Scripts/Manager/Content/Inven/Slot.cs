using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    #region UI
    public Image m_ItemImg;
    public Text m_ItemCount;
    public Button m_SlotBtn; //사용 및 장착을 위한 버튼(왼쪽 클릭시 사용, 오른쪽 클릭시 장착)
    int m_SlotNum; //슬롯 번호
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
