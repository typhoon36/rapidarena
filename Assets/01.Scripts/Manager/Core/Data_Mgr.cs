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
    public int Points = 99999;
}

[System.Serializable]
public class InvenSlot //�κ��丮 ������ �����ϱ����� Ŭ����
{
    public int SlotID;
    public int ItemID;
    public int ItemCount;
    public string ImagePath;
}
#endregion

// ���� �� ������ ����
public class Data_Mgr
{
    public TextAsset m_ItemData; //Json ���� ����
    public static UserData m_UserData = new UserData(); //���� ������
    public static List<ItemData> ItemOrder = new List<ItemData>(); //������ ����Ʈ
    public static List<InvenSlot> InvenSlots = new List<InvenSlot>(); // �κ��丮 ���� ����Ʈ

    public static void LoadData()
    {
        //������ �ε�
        string filePath = Application.dataPath + "/Resources/ItemData.json";
        if (File.Exists(filePath))
        {
            string FromJsonData = File.ReadAllText(filePath);//Json ���� �б�
            AllData allData = JsonUtility.FromJson<AllData>(FromJsonData);//Json ������ �迭�� AllData�� ��ȯ
            ItemOrder = new List<ItemData>(allData.Sheet1); // ������ ������ �ε�{Sheet1�� Json���ϰ� ���� �̸�}
            // �κ��丮 ���� ������ �ε�
            InvenSlots = allData.InvenSlots != null ? new List<InvenSlot>(allData.InvenSlots) : new List<InvenSlot>();
            //���� �����ڷ� null üũ�� �ε��ϴ� ���
        }

        UserData a_UserData = new UserData();
        a_UserData.UserName = PlayerPrefs.GetString("UserName");
        a_UserData.UserID = PlayerPrefs.GetString("UserID");
        a_UserData.Points = PlayerPrefs.GetInt("Points");
    }

    public static void SaveData()
    {
        // ������ ������ ����
        string filePath = Application.dataPath + "/Resources/ItemData.json";
        //��� ������ �ʱ�ȭ 
        AllData allData = new AllData();
        //���� ������ ��� ����
        allData.User = new UserData[] { m_UserData };
        //������ ������ ����
        allData.Sheet1 = ItemOrder.ToArray();
        // �κ��丮 ���� ������ ����
        allData.InvenSlots = InvenSlots.ToArray();

        string ToJsonData = JsonUtility.ToJson(allData, true);
        File.WriteAllText(filePath, ToJsonData);
        Debug.Log("Data Save Success");
    }

    public static List<ItemData> GetItemData()
    {
        return ItemOrder;
    }
}

