using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Equip_Slot : MonoBehaviour
{
    [HideInInspector] public int m_ESlotID; // 슬롯 ID 추가
    public Image m_ItemImg; // 아이템 이미지
    public Image m_SlotImg; // 슬롯 이미지

    #region Singleton
    public static Equip_Slot Inst;
    void Awake()
    {
        Inst = this;
    }
    #endregion

    //Rifle참조
    AsGun_Ctrl m_Gun;

    void Start()
    {
        //아직 장착이 되지않았으니 아이템 이미지는 비활성화
        m_ItemImg.gameObject.SetActive(false);
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            //우클릭시 장착된 아이템을 해제
            ClearESlot();
        }
    }

    public void ClearESlot()
    {

    }

    public void AddEItem(ItemData a_Data)
    {
        if (a_Data.ItemID == 5)
        {
            SendChangeSkin(m_Gun);
        }
    }

    //스킨을 변경시켜달라고 AsGun_Ctrl에게 알림
    public void SendChangeSkin(AsGun_Ctrl a_Gun)
    {
        a_Gun.ChangeSkin();
    }



}
