using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Product_Nd : MonoBehaviour
{
    public Text itemNameText;
    public Text amountText;

    public void SetItemData(ItemData itemData)
    {
        itemNameText.text = itemData.ItemName;
        amountText.text = itemData.Amount.ToString();
    }
}
