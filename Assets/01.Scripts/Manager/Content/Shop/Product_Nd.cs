using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Product_Nd : MonoBehaviour
{
    [HideInInspector] public ItemData m_ItemData;

    public Image m_ItemImg;
    public Text m_ItemNameTxt;
    public Text m_CountTxt;
    public Text m_PriceTxt;
    public Button m_Buy_Btn;


     Desc_Nd m_DescNd;


    void Start()
    {
        // Desc_Nd 인스턴스 찾기
        m_DescNd = GameObject.FindObjectOfType<Desc_Nd>();

        //상품 자체를 클릭했을 때
        Button a_Btn = GetComponent<Button>();
        if (a_Btn != null)
            a_Btn.onClick.AddListener(() => m_DescNd.SetItem(m_ItemData));



        if (m_Buy_Btn != null)
            m_Buy_Btn.onClick.AddListener(() =>
            {
                //Debug.Log("BuyBtn Clicked");
                Shop_Mgr a_ShopMgr = GameObject.FindObjectOfType<Shop_Mgr>();
                if (a_ShopMgr != null)
                {
                    a_ShopMgr.BuyItem(m_ItemData);
                }
            });
    }

    public void InitData(ItemData a_Data)
    {
        m_ItemData = a_Data;
        m_ItemImg.sprite = Resources.Load<Sprite>(a_Data.ImagePath);
        m_ItemNameTxt.text = a_Data.ItemName;
        m_CountTxt.text = a_Data.Amount.ToString();
        m_PriceTxt.text = a_Data.ItemPrice.ToString();
    }

    public void RefreshState()
    {
        m_ItemImg.sprite = Resources.Load<Sprite>(m_ItemData.ImagePath);
        m_ItemNameTxt.text = m_ItemData.ItemName;
        m_CountTxt.text = m_ItemData.Amount.ToString();
        m_PriceTxt.text = m_ItemData.ItemPrice.ToString();
    }
}
