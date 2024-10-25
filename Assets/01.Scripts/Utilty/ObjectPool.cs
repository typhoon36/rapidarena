using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    //������ ������ų�������� ���� ����
    class PoolItem
    {
        public GameObject Obj;
        public bool IsActive;
    }

    int IncreaseCnt = 5;
    int maxCnt;
    int activeCnt;

    GameObject PoolObject;
    List<PoolItem> PoolList;

    int MaxCnt => maxCnt;
    int ActiveCnt => activeCnt;

    public void Initialize(GameObject _poolObj)
    {
        maxCnt = 0;
        activeCnt = 0;

        PoolObject = _poolObj;

        PoolList = new List<PoolItem>();

        InstantiateObjects();
    }

    ///<summary>
    ///������Ʈ �̸� ����
    ///</summary>
    public void InstantiateObjects()
    {
        maxCnt += IncreaseCnt;

        for (int i = 0; i < IncreaseCnt; i++)
        {
            PoolItem a_Item = new PoolItem();

            a_Item.IsActive = false;
            a_Item.Obj = GameObject.Instantiate(PoolObject);
            a_Item.Obj.SetActive(false);

            PoolList.Add(a_Item);
        }
    }

    ///<summary>
    /// ���� �������� ������Ʈ ����
    /// </summary>
    public void DestroyObjects()
    {
        if (PoolList == null) return;

        int cnt = PoolList.Count;

        for (int i = 0; i < cnt; i++)
        {
            Destroy(PoolList[i].Obj);
        }
    }

    ///<summary>
    /// ����Ʈ�� ����� ������Ʈ�� Ȱ��ȭ
    /// ���� ������̸� �߰� ����
    /// </summary>
    public GameObject ActivateItem()
    {
        if (PoolList == null) return null;

        if (maxCnt == activeCnt)
        {
            InstantiateObjects();
        }

        int cnt = PoolList.Count;
        for (int i = 0; i < cnt; i++)
        {
            PoolItem a_Item = PoolList[i];

            if (a_Item.IsActive == false)
            {
                activeCnt++;

                a_Item.IsActive = true;
                a_Item.Obj.SetActive(true);

                return a_Item.Obj;
            }
        }

        return null;
    }

    /// <summary>
    ///  ���� ����� ���� ������Ʈ ��Ȱ��ȭ
    /// </summary>
    public void DeactivateItem(GameObject a_obj)
    {
        if (PoolList == null || a_obj == null) return;

        int cnt = PoolList.Count;

        for (int i = 0; i < cnt; i++)
        {
            PoolItem a_Item = PoolList[i];

            if (a_Item.Obj == a_obj)
            {
                activeCnt--;

                a_Item.IsActive = false;
                a_Item.Obj.SetActive(false);

                return;
            }
        }
    }

    ///<summary>
    /// ������� ��� ������Ʈ ��Ȱ��ȭ
    ///  </summary>
    public void DeactivateAllItem()
    {
        if (PoolList == null) return;

        int cnt = PoolList.Count;

        for (int i = 0; i < cnt; i++)
        {
            PoolItem a_Item = PoolList[i];

            if (a_Item.Obj != null && a_Item.IsActive == true)
            {
                a_Item.IsActive = false;
                a_Item.Obj.SetActive(false);
            }
        }

        activeCnt = 0;
    }
}
