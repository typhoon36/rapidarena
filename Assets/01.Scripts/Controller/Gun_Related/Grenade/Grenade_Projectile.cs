using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade_Projectile : MonoBehaviour
{
    [Header("Explosion")]
    [SerializeField] private GameObject m_ExplosionEff;
    [SerializeField] private float m_ExplosionRadius = 10f;
    [SerializeField] private float m_ExplosionPower = 500.0f;
    [SerializeField] private int m_throwForce = 1000;
    [SerializeField] private float m_ExplosionDelay = 2.0f; // 폭발 지연 시간

    int m_ExDmg;
    new Rigidbody rigidbody;

    public void SetUp(int a_Dmg, Vector3 a_Rot)
    {
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.AddForce(a_Rot * m_throwForce);

        m_ExDmg = a_Dmg;

        // 폭발 지연 시간 후 폭발
        StartCoroutine(ExplodeAfterDelay());
    }

    private IEnumerator ExplodeAfterDelay()
    {
        yield return new WaitForSeconds(m_ExplosionDelay);
        Explode();
    }

    private void Explode()
    {
        GameObject explosion = Instantiate(m_ExplosionEff, transform.position, transform.rotation);

        //폭발 반경 내의 모든 콜라이더를 가져옴
        Collider[] colls = Physics.OverlapSphere(transform.position, m_ExplosionRadius);

        foreach (Collider hit in colls)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(m_ExplosionPower, transform.position, m_ExplosionRadius);
            }
        }

        Destroy(gameObject);

        // 2초 후 폭발 효과 제거
        Destroy(explosion, 2.0f);
    }
}
