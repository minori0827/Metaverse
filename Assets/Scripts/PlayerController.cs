using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("�ړ����x")] public float speed;
    [Header("�J�������x")] public float cameraSpeed;
    [Header("�J����")] public Transform cameraParent;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        Movement();
        CameraMove();
    }

    void Movement()
    {
        float horizontalKey = Input.GetAxis("Horizontal");
        float verticalKey = Input.GetAxis("Vertical");

        float xDirection = 0;
        float zDirection = 0;
        float xSpeed = 0;
        float zSpeed = 0;

        if (Input.GetKey(KeyCode.W)) //���ړ�
        {
            zDirection = 1;
        }
        else if (Input.GetKey(KeyCode.S)) //��O�ړ�
        {
            zDirection = -1;
        }

        if (Input.GetKey(KeyCode.D)) //�E�ړ�
        {
            xDirection = 1;
        }
        else if (Input.GetKey(KeyCode.A)) //���ړ�
        {
            xDirection = -1;
        }

        if (xDirection != 0 || zDirection != 0)
        {
            xSpeed = speed * xDirection / Mathf.Sqrt(Mathf.Pow(xDirection, 2) + Mathf.Pow(zDirection, 2));
            zSpeed = speed * zDirection / Mathf.Sqrt(Mathf.Pow(xDirection, 2) + Mathf.Pow(zDirection, 2));
        }

        rb.velocity = new Vector3(xSpeed, 0, zSpeed);
    }

    void CameraMove()
    {
        Vector3 eulerAngles = cameraParent.eulerAngles;
        if (Input.GetKey(KeyCode.E)) //�J�����E�ړ�
        {
            eulerAngles.y -= cameraSpeed;
        }
        else if (Input.GetKey(KeyCode.Q)) //�J�������ړ�
        {
            eulerAngles.y += cameraSpeed;
        }
        cameraParent.eulerAngles = eulerAngles;
    }
}
