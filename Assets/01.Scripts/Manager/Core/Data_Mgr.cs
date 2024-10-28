using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;

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
}

[System.Serializable]
public class UserData
{
    public string UserName;
    public string UserID;
    public int Points;
}
#endregion

// Data Manager
public class Data_Mgr : MonoBehaviour
{
    public TextAsset m_ItemData;
    public GameObject productPrefab;
    public Transform productParent;

    AllItemData Itemdatas;
    UserData userData = new UserData();

    void Awake()
    {
        // Load Data from Json
        Itemdatas = JsonUtility.FromJson<AllItemData>(m_ItemData.text);

        foreach (var item in Itemdatas.Sheet1)
        {
            GameObject product = Instantiate(productPrefab, productParent);
            Product_Nd productNd = product.GetComponent<Product_Nd>();
            productNd.SetItemData(item);
        }
    }

    void SaveDataToJson()
    {
        string json = JsonUtility.ToJson(userData);
        File.WriteAllText(Application.persistentDataPath + "/UserData.json", json);
    }

    void LoadDataFromJson()
    {
        string path = Application.persistentDataPath + "/UserData.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            userData = JsonUtility.FromJson<UserData>(json);
            Debug.Log($"UserName: {userData.UserName}, UserID: {userData.UserID}, Points: {userData.Points})");
        }
    }
}
