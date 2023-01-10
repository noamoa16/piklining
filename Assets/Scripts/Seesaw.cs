using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seesaw : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // �V�[�\�[�������Ȃ��悤�ɒ���
        float zAngle = transform.eulerAngles.z;
        if (zAngle > 180) zAngle -= 360;
        if (Math.Abs(zAngle) < 60)
        {
            GetComponent<Rigidbody2D>().AddForce(new Vector2(
                -transform.position.x * (2500 - Mathf.Pow(zAngle, 2)),
            0));
        }

        // �V�[�\�[�̗���
        // ������
        if (transform.position.y < -20)
        {
            Destroy(gameObject);
        }
    }
}
