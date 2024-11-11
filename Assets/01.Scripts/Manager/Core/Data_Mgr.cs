using UnityEngine;
using System.IO;
using System.Collections.Generic;

// 게임 내 데이터 관리
public class Data_Mgr
{
    #region Singleton
    private static Data_Mgr _Inst;
    public static Data_Mgr Inst
    {
        get
        {
            if (_Inst == null)
            {
                _Inst = new Data_Mgr();
            }
            return _Inst;
        }
    }
    #endregion

    public TextAsset m_ItemData;
    AllItemData Itemdata;
    public UserData userData = new UserData();
    public List<ItemData> m_Items = new List<ItemData>();


    Data_Mgr()
    {
        LoadItemData();
        LoadUserDataFromJson();
    }

    public void LoadItemData()
    {
        if (m_ItemData == null)
            m_ItemData = Resources.Load<TextAsset>("ItemData");

        Itemdata = JsonUtility.FromJson<AllItemData>(m_ItemData.text);
    }

    public AllItemData GetItemData()
    {
        return Itemdata;
    }

    public bool CanAfford(int price)
    {
        return userData.Points >= price;
    }

    public void DeductPoints(int price)
    {
        if (CanAfford(price))
        {
            userData.Points -= price;
            SaveUserDataToJson();
        }
    }

    #region User Data to Json
    void SaveUserDataToJson()
    {
        string json = JsonUtility.ToJson(userData);
        File.WriteAllText(Application.persistentDataPath + "/UserData.json", json);
    }

    void LoadUserDataFromJson()
    {
        string path = Application.persistentDataPath + "/UserData.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            userData = JsonUtility.FromJson<UserData>(json);
        }
        userData.Points = 99999;
    }
    #endregion

    public void SortItems()
    {
        m_Items.Sort((x, y) => x.ItemID.CompareTo(y.ItemID));
    }
}


//Jsons Data
#region Data Structure
[System.Serializable]
public class AllItemData
{
    public ItemData[] Sheet1;
    public UserData[] User;
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
    public int Points;
}

[System.Serializable]
public class Serialization<T>
{
    public List<T> target;
    public Serialization(List<T> target)
    {
        this.target = target;
    }

    public List<T> ToList()
    {
        return target;
    }
}

[System.Serializable]
public class SlotState
{
    public int SlotIndex;
    public ItemData ItemData;
}
#endregion

