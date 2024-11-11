using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

//필드 아이템(탄창 회복)
public class Item_Mag : ItemBase
{
    [SerializeField] private GameObject m_Eff;
    [SerializeField] private int m_IncreaseBullet = 2;
    [SerializeField] private float m_Movedist = 0.2f;
    [SerializeField] private float m_PingPongSpeed = 0.5f;
    [SerializeField] private float m_RotSpeed = 50;

    IEnumerator Start()
    {
        float y = transform.position.y;

        while (true)
        {
            // y축 기준 회전
            transform.Rotate(Vector3.up * m_RotSpeed * Time.deltaTime);

            Vector3 a_Pos = transform.position;
            a_Pos.y = Mathf.Lerp(y, y + m_Movedist, Mathf.PingPong(Time.time * m_PingPongSpeed, 1));
            transform.position = a_Pos;

            yield return null;
        }
    }

    public override void Use(GameObject _Entity)
    {
        Base_Ctrl baseCtrl = _Entity.GetComponentInChildren<Base_Ctrl>();
        if (baseCtrl != null)
        {
            baseCtrl.IncreaseMag(m_IncreaseBullet);
            Instantiate(m_Eff, transform.position, Quaternion.identity);
        }
    }

    void OnTriggerEnter(Collider coll)
    {
        if (coll.CompareTag("Player"))
        {
            Use(coll.gameObject);
        }
    }


}
