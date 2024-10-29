using UnityEngine;
using System.IO;



// Data Manager
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

    AllItemData Itemdatas;
    UserData userData = new UserData();

    private Data_Mgr()
    {
        // Load Data from Json
        LoadItemData();
        LoadUserDataFromJson();
    }

    public void LoadItemData()
    {
        if (m_ItemData == null)
        {
            // 리소스에서 m_ItemData 로드
            m_ItemData = Resources.Load<TextAsset>("ItemData");
        }

        Itemdatas = JsonUtility.FromJson<AllItemData>(m_ItemData.text);
    }

    public AllItemData GetItemData()
    {
        return Itemdatas;
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
    }
}


// Json Data Structure
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
#endregion

