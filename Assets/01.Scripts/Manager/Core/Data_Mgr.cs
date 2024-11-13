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
public class InvenSlot //인벤토리 슬롯을 저장하기위한 클래스
{
    public int SlotID;
    public int ItemID;
    public int ItemCount;
    public string ImagePath;
}
#endregion

// 게임 내 데이터 관리
public class Data_Mgr
{
    public TextAsset m_ItemData; //Json 파일 변수
    public static UserData m_UserData = new UserData(); //유저 데이터
    public static List<ItemData> ItemOrder = new List<ItemData>(); //아이템 리스트
    public static List<InvenSlot> InvenSlots = new List<InvenSlot>(); // 인벤토리 슬롯 리스트

    public static void LoadData()
    {
        //데이터 로드
        string filePath = Application.dataPath + "/Resources/ItemData.json";
        if (File.Exists(filePath))
        {
            string FromJsonData = File.ReadAllText(filePath);//Json 파일 읽기
            AllData allData = JsonUtility.FromJson<AllData>(FromJsonData);//Json 파일을 배열인 AllData로 변환
            ItemOrder = new List<ItemData>(allData.Sheet1); // 아이템 데이터 로드{Sheet1은 Json파일과 맞춘 이름}
            // 인벤토리 슬롯 데이터 로드
            InvenSlots = allData.InvenSlots != null ? new List<InvenSlot>(allData.InvenSlots) : new List<InvenSlot>();
            //삼항 연산자로 null 체크후 로드하는 방식
        }

        UserData a_UserData = new UserData();
        a_UserData.UserName = PlayerPrefs.GetString("UserName");
        a_UserData.UserID = PlayerPrefs.GetString("UserID");
        a_UserData.Points = PlayerPrefs.GetInt("Points");
    }

    public static void SaveData()
    {
        // 아이템 데이터 저장
        string filePath = Application.dataPath + "/Resources/ItemData.json";
        //모든 데이터 초기화 
        AllData allData = new AllData();
        //유저 데이터 골라 저장
        allData.User = new UserData[] { m_UserData };
        //아이템 데이터 저장
        allData.Sheet1 = ItemOrder.ToArray();
        // 인벤토리 슬롯 데이터 저장
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

