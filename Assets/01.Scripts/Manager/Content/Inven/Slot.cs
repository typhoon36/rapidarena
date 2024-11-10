using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    public Image m_ItemImg;
    public Text m_ItemCount;
    ItemData m_ItemData;

    #region Singleton
    public static Slot Inst;
    void Awake()
    {
        Inst = this;
    }
    #endregion


    public void AddItem(ItemData _itemData)
    {
        m_ItemData = _itemData;
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
        m_ItemImg.sprite = null;
        m_ItemImg.gameObject.SetActive(false);
        m_ItemCount.text = "";
        m_ItemCount.gameObject.SetActive(false);
    }
}
