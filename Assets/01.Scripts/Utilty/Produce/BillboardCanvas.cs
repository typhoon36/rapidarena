using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillboardCanvas : MonoBehaviour
{
    Transform mainCameraTr;

    // Start is called before the first frame update
    void Start()
    {
        //���������� �ִ� ���� ī�޶��� Transform ������Ʈ�� ����
        mainCameraTr = Camera.main.transform;
    }

    //// Update is called once per frame
    //void Update()
    //{

    //}

    void LateUpdate()
    {
        //���ʿ��� ����� ��� ������ �ʰ� Hp�ٰ� ĳ���͸� ����ٴѴ�.
        transform.forward = mainCameraTr.forward;
        //Quad�� ���� ����� ������ �ϴ� ���
    }
}
