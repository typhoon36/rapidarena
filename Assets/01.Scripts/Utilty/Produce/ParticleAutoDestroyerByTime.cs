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
        // 파티클이 재생중이 아니면 삭제
        if (particle.isPlaying == false)
        {
            Destroy(gameObject);
        }
    }
}

