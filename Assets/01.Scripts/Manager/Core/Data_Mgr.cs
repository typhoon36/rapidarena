using UnityEngine;
using System.IO;
using System.Collections.Generic;


//Jsons Data
#region Data Structure
[System.Serializable]
public class AllData
{
    public ItemData[] Sheet1;
    public UserData[] User;
    public InvenSlot[] InvenSlots;
}

[System.Serializable]
public class ItemData
{
    public int ItemID;
    public string ItemName;
    public string ItemType;
    public string AmmoType;
    public int Amount;
    public int ItemPrice;
    public string ItemDescription;
    public string ImagePath;
}

[System.Serializable]
public class UserData
{
    public string UserName;
    public string UserID;
    public int Points = 999;
}

[System.Serializable]
public class InvenSlot
{
    public int SlotID;
    public int ItemID;
    public int ItemCount;
    public string ImagePath;
}
#endregion

public class Data_Mgr
{
    public TextAsset m_ItemData;
    public static UserData m_UserData = new UserData();
    public static List<ItemData> ItemOrder = new List<ItemData>();
    public static List<InvenSlot> InvenSlots = new List<InvenSlot>();

    public static void LoadData()
    {
        TextAsset jsonData = Resources.Load<TextAsset>("ItemData");
        if (jsonData != null)
        {
            string FromJsonData = jsonData.text;
            AllData allData = JsonUtility.FromJson<AllData>(FromJsonData);
            ItemOrder = new List<ItemData>(allData.Sheet1);
            InvenSlots = allData.InvenSlots != null ? new List<InvenSlot>(allData.InvenSlots) : new List<InvenSlot>();
        }

        m_UserData.UserName = PlayerPrefs.GetString("UserName", "DefaultUserName");
        m_UserData.UserID = PlayerPrefs.GetString("UserID", "DefaultUserID");
        m_UserData.Points = PlayerPrefs.GetInt("Points", 999);
    }

    public static void SaveData()
    {
        AllData allData = new AllData();
        allData.User = new UserData[] { m_UserData };
        allData.Sheet1 = ItemOrder.ToArray();
        allData.InvenSlots = InvenSlots.ToArray();

        string ToJsonData = JsonUtility.ToJson(allData, true);
        string filePath = Application.dataPath + "/Resources/ItemData.json";
        File.WriteAllText(filePath, ToJsonData);

        PlayerPrefs.SetString("UserName", m_UserData.UserName);
        PlayerPrefs.SetString("UserID", m_UserData.UserID);
        PlayerPrefs.SetInt("Points", m_UserData.Points);
        PlayerPrefs.Save();
    }

    public static List<ItemData> GetItemData()
    {
        return ItemOrder;
    }
}

