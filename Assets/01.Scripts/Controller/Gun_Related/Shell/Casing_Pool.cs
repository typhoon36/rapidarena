using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Casing_Pool : MonoBehaviour
{
    [SerializeField]
    private GameObject m_CasingPrefab;
    private ObjectPool m_Pool;

    void Awake()
    {
        // ObjectPool 컴포넌트를 추가하고 초기화
        m_Pool = gameObject.AddComponent<ObjectPool>();
        m_Pool.Initialize(m_CasingPrefab);
    }

    public void SpawnCase(Vector3 a_Pos, Vector3 a_Dir)
    {
        GameObject a_Item = m_Pool.ActivateItem();
        a_Item.transform.position = a_Pos;
        a_Item.transform.rotation = Random.rotation;
        a_Item.transform.parent = this.transform; 
        a_Item.GetComponent<Casing_Ctrl>().Setup(m_Pool, a_Dir);
    }
}
