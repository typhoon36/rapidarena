using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillboardCanvas : MonoBehaviour
{
    Transform mainCameraTr;

    void Start()
    {
        mainCameraTr = Camera.main.transform;
    }


    void LateUpdate()
    {
        transform.forward = mainCameraTr.forward;
    }
}
