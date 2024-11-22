using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_Ctrl : MonoBehaviour
{
    //�Ѿ� �ӵ�
    public float speed = 6000.0f;

    //������
    public int damage = 20;

    public GameObject hiteff;

    private CapsuleCollider _collider;

    private Rigidbody _rigidbody;

    [HideInInspector] public int AttackerId = -1;
    [HideInInspector] public string AttackerTeam = "blue"; //��������� �� �Ѿ�����?


    void Start()
    {
        _collider = GetComponent<CapsuleCollider>();
        _rigidbody = GetComponent<Rigidbody>();

        // �Ѿ��� �� �������� ���� ���մϴ�.
        _rigidbody.AddForce(transform.forward * speed);

        // 3�ʰ� ���� �� �ڵ� �����ϴ� �ڷ�ƾ ����
        StartCoroutine(this.Spark(3.0f));
    }

    void OnTriggerEnter(Collider coll)
    {
        if (coll.tag == "Bullet") return;
        else if (coll.tag == "Item") return;
        else if (coll.tag == "Muzzle") return;

        // �� �Ǵ� ������ ������� �ڷ�ƾ ����
        StartCoroutine(this.Spark(0.1f));
    }

    IEnumerator Spark(float tm)
    {
        yield return new WaitForSeconds(tm);
        //�浹 �ݺ� �Լ��� �߻����� �ʵ��� Collider�� ��Ȱ��ȭ
        if (_collider != null)
            _collider.enabled = false;

        //���������� ������ ���� �ʿ� ����
        if (_rigidbody != null)
        {
            _rigidbody.collisionDetectionMode = CollisionDetectionMode.Discrete;
            _rigidbody.isKinematic = false;
            _rigidbody.velocity = new Vector3(0, 0, 0);
            _rigidbody.isKinematic = true;
        }


        //���� ������ ���� ����
        GameObject obj = (GameObject)Instantiate(hiteff,
                                transform.position - (transform.forward),
                                Quaternion.identity);

        Destroy(obj, 1.0f);

        //������ ������ �Ѿ� ����
        Destroy(this.gameObject, 0.1f);
    }


}
