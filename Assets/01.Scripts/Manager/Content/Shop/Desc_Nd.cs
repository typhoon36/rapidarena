using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Desc_Nd : MonoBehaviour
{
    public Text itemNameText;
    public Text itemDescpText;
    public Image itemImage;

    void Start()
    {
        // �ʱ�ȭ
        itemNameText.text = "";
        itemDescpText.text = "";
        itemImage.gameObject.SetActive(false);
    }

    public void SetItemData(ItemData itemData)
    {
        itemNameText.text = "<" + itemData.ItemName + ">";
        itemDescpText.text = itemData.ItemDescription;

        if (!string.IsNullOrEmpty(itemData.ImagePath))
        {
            Sprite sprite = Resources.Load<Sprite>(itemData.ImagePath);
            if (sprite != null)
            {
                itemImage.sprite = sprite;
                itemImage.gameObject.SetActive(true);
            }
            else
            {
                Debug.LogWarning("�̹����� �ε��� �� �����ϴ�: " + itemData.ImagePath);
                itemImage.gameObject.SetActive(false);
            }
        }
        else
        {
            itemImage.gameObject.SetActive(false);
        }
    }
}
