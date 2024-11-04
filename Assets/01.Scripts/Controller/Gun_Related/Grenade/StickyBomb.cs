using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickyBomb : MonoBehaviour
{
    public GameObject BombObj;

    public void SpawnBomb()
    {
        Vector3 SpawnPos = new Vector3(27f, 0.1f, 8f);
        Instantiate(BombObj, SpawnPos, Quaternion.identity);
    }

}
