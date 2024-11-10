using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Product_Nd : MonoBehaviour
{
    #region UI
    public Text m_ItemNameTxt;
    public Text m_AmountTxt;
    public Text m_ItemPriceTxt;
    public Button buyBtn;

    public Image m_ItemImg;
    #endregion

    #region Reference
    private Desc_Nd m_DescNd;
    private Data_Mgr m_DataMgr;
    private Shop_Mgr m_ShopMgr;
    private ItemData m_CurData;
    #endregion


    void Awake()
    {
        m_DescNd = FindObjectOfType<Desc_Nd>();
        m_DataMgr = Data_Mgr.Inst;
        m_ShopMgr = Shop_Mgr.Inst;
    }

    void Start()
    {
        if (buyBtn != null)
            buyBtn.onClick.AddListener(() => Buy());
    }

    //상품 정보 설정
    public void SetItemData(ItemData itemData)
    {
        m_ItemNameTxt.text = itemData.ItemName;
        // 글자 크기 조정
        if (itemData.ItemName == "SkinnedBox")
        {
            m_ItemNameTxt.fontSize = 17;
        }
        else if (itemData.ItemName == "Weapon_Kit")
        {
            m_ItemNameTxt.fontSize = 17;
        }
        else
        {
            m_ItemNameTxt.fontSize = 19;
        }
 
        m_AmountTxt.text = itemData.Amount.ToString();
        m_ItemPriceTxt.text = itemData.ItemPrice.ToString();
        m_CurData = itemData;

        // 이미지 로드
        if (!string.IsNullOrEmpty(itemData.ImagePath))
        {
            Sprite sprite = LoadSprite(itemData.ImagePath);
            if (sprite != null)
            {
                m_ItemImg.sprite = sprite;
            }
        }
    }

    public void OnItemClick(ItemData itemData)
    {
        if (m_DescNd != null)
            m_DescNd.SetItemData(itemData);
    }

    // 구매
    void Buy()
    {
        if (m_DataMgr != null && m_ShopMgr != null)
        {
            if (m_DataMgr.CanAfford(m_CurData.ItemPrice))
            {
                m_DataMgr.DeductPoints(m_CurData.ItemPrice);
                m_ShopMgr.ShowMsg("구매 성공!");
            }
            else
            {
                m_ShopMgr.ShowMsg("포인트가 부족합니다.");
            }
        }
    }

    // LoadSprite 메서드 추가
    private Sprite LoadSprite(string path)
    {
        Texture2D texture = Resources.Load<Texture2D>(path);
        if (texture != null)
        {
            return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        }
        return null;
    }
}
