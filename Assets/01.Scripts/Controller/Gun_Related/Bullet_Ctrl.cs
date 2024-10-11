using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_Ctrl : MonoBehaviour
{
    #region �Ѿ� �ӵ� �� �ı���
    public float speed = 20.0f;
    public int damage = 10;
    #endregion

    //���� �¾������� ����Ʈ
    public GameObject SparkEffect;

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

        else if (coll.collider.tag == "Player") return;

        else if(coll.collider.tag == "Item") return;

        else if (coll.collider.includeLayers == 1 << LayerMask.NameToLayer("Default")) return;


        GameObject Spark = Instantiate(SparkEffect, transform.position, Quaternion.identity);

        Destroy(Spark, Spark.GetComponent<ParticleSystem>().main.duration + 0.2f);

        Destroy(gameObject);
    }
}
