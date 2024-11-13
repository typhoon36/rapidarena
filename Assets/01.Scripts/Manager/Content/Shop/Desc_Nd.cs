using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Desc_Nd : MonoBehaviour
{
    public Text m_NameText;
    public Text m_DescText;
    public Image m_ItemImg;

    void Start()
    {
        // 초기화
        m_NameText.text = "";
        m_DescText.text = "";
        m_ItemImg.gameObject.SetActive(false);
    }

    //아이템 정보 설정(상품 클릭시 호출)
    public void SetItem(ItemData a_Data)
    {
        m_NameText.text = "<" + a_Data.ItemName + ">";
        m_DescText.text = a_Data.ItemDescription;

        if (string.IsNullOrEmpty(a_Data.ImagePath) == true)
        {
            m_ItemImg.gameObject.SetActive(false);
        }
        else
        {
            Sprite a_Sprite = Resources.Load<Sprite>(a_Data.ImagePath);
            if (a_Sprite != null)
            {
                m_ItemImg.sprite = a_Sprite;
                m_ItemImg.gameObject.SetActive(true);
            }
            else
            {
                m_ItemImg.gameObject.SetActive(false);
                Debug.LogError("Image Load Fail : " + a_Data.ImagePath);
            }

        }


    }

}
