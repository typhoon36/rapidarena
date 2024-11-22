using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_Ctrl : MonoBehaviour
{
    //총알 속도
    public float speed = 6000.0f;

    //데미지
    public int damage = 20;

    public GameObject hiteff;

    private CapsuleCollider _collider;

    private Rigidbody _rigidbody;

    [HideInInspector] public int AttackerId = -1;
    [HideInInspector] public string AttackerTeam = "blue"; //어느팀에서 쏜 총알인지?


    void Start()
    {
        _collider = GetComponent<CapsuleCollider>();
        _rigidbody = GetComponent<Rigidbody>();

        // 총알의 앞 방향으로 힘을 가합니다.
        _rigidbody.AddForce(transform.forward * speed);

        // 3초가 지난 후 자동 폭발하는 코루틴 실행
        StartCoroutine(this.Spark(3.0f));
    }

    void OnTriggerEnter(Collider coll)
    {
        if (coll.tag == "Bullet") return;
        else if (coll.tag == "Item") return;
        else if (coll.tag == "Muzzle") return;

        // 벽 또는 적에게 닿은경우 코루틴 실행
        StartCoroutine(this.Spark(0.1f));
    }

    IEnumerator Spark(float tm)
    {
        yield return new WaitForSeconds(tm);
        //충돌 콜벡 함수가 발생하지 않도록 Collider를 비활성화
        if (_collider != null)
            _collider.enabled = false;

        //물리엔진의 영향을 받을 필요 없음
        if (_rigidbody != null)
        {
            _rigidbody.collisionDetectionMode = CollisionDetectionMode.Discrete;
            _rigidbody.isKinematic = false;
            _rigidbody.velocity = new Vector3(0, 0, 0);
            _rigidbody.isKinematic = true;
        }


        //폭발 프리팹 동적 생성
        GameObject obj = (GameObject)Instantiate(hiteff,
                                transform.position - (transform.forward),
                                Quaternion.identity);

        Destroy(obj, 1.0f);

        //폭발후 스폰한 총알 삭제
        Destroy(this.gameObject, 0.1f);
    }


}
