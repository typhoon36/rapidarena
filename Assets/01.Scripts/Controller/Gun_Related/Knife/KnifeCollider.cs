using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnifeCollider : MonoBehaviour
{
    private new Collider collider;
    private int m_Damage;
    public Knife_Ctrl knifeCtrl; // Knife_Ctrl 참조 추가

    private void Awake()
    {
        collider = GetComponent<Collider>();
        collider.enabled = false;
    }

    public void StartCollider(int damage)
    {
        m_Damage = damage;
        collider.enabled = true;
        StartCoroutine(DisableByTime(0.1f));
    }

    IEnumerator DisableByTime(float time)
    {
        yield return new WaitForSeconds(time);
        collider.enabled = false;
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Muzzle")) return;
        if (collision.gameObject.CompareTag("Player")) return; // 플레이어 태그 무시
        if (collision.gameObject.CompareTag("Item")) return;
        if (collision.gameObject.CompareTag("Untagged")) return; // 태그 없는 오브젝트 무시

        if (collision.gameObject.CompareTag("Wall") && knifeCtrl.IsAttack)
        {
            knifeCtrl.SpawnEffect(); // Knife_Ctrl에서 이펙트 생성 호출
        }
    }
}
