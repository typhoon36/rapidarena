using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    //무엇을 관리시킬것인지에 대한 정보
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
    ///오브젝트 미리 생성
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
    /// 현재 관리중인 오브젝트 삭제
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
    /// 리스트에 저장된 오브젝트를 활성화
    /// 만약 사용중이면 추가 생성
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
    ///  현재 사용이 끝난 오브젝트 비활성화
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
    /// 사용중인 모든 오브젝트 비활성화
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
