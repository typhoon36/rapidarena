using UnityEngine;

public class ParticleAutoDestroyerByTime : MonoBehaviour
{
    ParticleSystem particle;

    void Awake()
    {
        particle = GetComponent<ParticleSystem>();
    }

    void Update()
    {
        // ��ƼŬ�� ������� �ƴϸ� ����
        if (particle.isPlaying == false)
        {
            Destroy(gameObject);
        }
    }
}

