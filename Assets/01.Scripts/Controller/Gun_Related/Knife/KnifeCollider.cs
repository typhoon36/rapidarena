using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnifeCollider : MonoBehaviour
{
    private new Collider collider;
    private int m_Damage;
    public Knife_Ctrl knifeCtrl; // Knife_Ctrl ���� �߰�

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
        if (collision.gameObject.CompareTag("Player")) return; // �÷��̾� �±� ����
        if (collision.gameObject.CompareTag("Item")) return;
        if (collision.gameObject.CompareTag("Untagged")) return; // �±� ���� ������Ʈ ����

        if (collision.gameObject.CompareTag("Wall") && knifeCtrl.IsAttack)
        {
            knifeCtrl.SpawnEffect(); // Knife_Ctrl���� ����Ʈ ���� ȣ��
        }
    }
}
