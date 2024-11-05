using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_Ctrl : MonoBehaviour
{
    #region �Ѿ� �ӵ� �� �ı���
    public float speed = 6000.0f;
    public int damage = 10;
    #endregion

    //���� �¾������� ����Ʈ
    public GameObject SparkEffect;

    //�߻��� �÷��̾��� ID ����
    [HideInInspector] public int AttackerId = -1;
    [HideInInspector] public string AttackerTeam = "blue";

    void Start()
    {
        speed = 3000.0f;

        if (gameObject.tag == "Bullet")
        {
            transform.forward = Camera.main.transform.forward;
        }
        GetComponent<Rigidbody>().AddForce(transform.forward * speed);

        Destroy(gameObject, 4.0f);
    }

    void OnCollisionEnter(Collision coll)
    {
        if (coll.collider.tag == "Muzzle") return;

        if (coll.collider.tag == "Bullet") return;

        if (coll.collider.tag == "Player")
        {
            // Damage Ŭ������ ȣ���Ͽ� HP ����
            Damage damageScript = coll.collider.GetComponent<Damage>();
            if (damageScript != null)
            {
                damageScript.TakeDamage(AttackerId, AttackerTeam);
            }

            Destroy(gameObject);
            return;
        }

        if (coll.collider.tag == "Item") return;

        // �浹�� ���� ������Ʈ�� �±װ� ��
        if (coll.collider.tag == "Wall")
        {
            GameObject Spark = Instantiate(SparkEffect, transform.position, Quaternion.identity);
            Destroy(Spark, Spark.GetComponent<ParticleSystem>().main.duration + 0.2f);

            Destroy(gameObject);
        }
    }
}
