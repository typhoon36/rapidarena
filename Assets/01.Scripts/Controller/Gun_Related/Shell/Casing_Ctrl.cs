using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Casing_Ctrl : MonoBehaviour
{
    [SerializeField]
    private float m_dTime = 5f;//ź�� ���� �� ������� �ð�

    [SerializeField]
    private float m_CSpin = 1.0f;//ź�� ȸ�� �ӵ�

    [SerializeField]
    private AudioClip[] audioClips;

    Rigidbody m_Rd;
    AudioSource m_Audio;
    ObjectPool m_Pool;

    public void Setup(ObjectPool a_Pool, Vector3 a_Dir)
    {
        m_Rd = GetComponent<Rigidbody>();
        m_Audio = GetComponent<AudioSource>();
        m_Pool = a_Pool;

        //ź�� �̵� �ӵ� & ȸ�� �ӵ�
        m_Rd.velocity = new Vector3(a_Dir.x, 1, a_Dir.z);
        m_Rd.angularVelocity = new Vector3(Random.Range(-m_CSpin, m_CSpin),
                                           Random.Range(-m_CSpin, m_CSpin),
                                           Random.Range(-m_CSpin, m_CSpin));

        //ź�� ��Ȱ��ȭ �ڷ�ƾ ����
        StartCoroutine(DeactivateAfterTime());
    }

    void OnCollisionEnter(Collision collision)
    {
        int Idx = Random.Range(0, audioClips.Length);
        m_Audio.clip = audioClips[Idx];
        m_Audio.Play();
    }

    IEnumerator DeactivateAfterTime()
    {
        yield return new WaitForSeconds(m_dTime);
        m_Pool.DeactivateItem(this.gameObject);
    }

}
