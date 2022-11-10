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

        float horizontalDirection = 0; //���E�̈ړ�����
        float verticalDirection = 0; //�O��̈ړ�����
        float horizontalSpeed = 0; //���E�̈ړ����x
        float verticalSpeed = 0; //�O��̈ړ����x

        if (Input.GetKey(KeyCode.W)) //���ړ�
        {
            verticalDirection = 1;
        }
        else if (Input.GetKey(KeyCode.S)) //��O�ړ�
        {
            verticalDirection  = -1;
        }

        if (Input.GetKey(KeyCode.D)) //�E�ړ�
        {
            horizontalDirection = 1;
        }
        else if (Input.GetKey(KeyCode.A)) //���ړ�
        {
            horizontalDirection = -1;
        }

        if (verticalDirection != 0 || horizontalDirection != 0)
        {
            verticalSpeed = speed * verticalDirection / Mathf.Sqrt(Mathf.Pow(verticalDirection, 2)
                                + Mathf.Pow(horizontalDirection, 2));
            horizontalSpeed = speed * horizontalDirection / Mathf.Sqrt(Mathf.Pow(verticalDirection, 2)
                                + Mathf.Pow(horizontalDirection, 2));
        }
        
        float xSpeed = verticalSpeed * Mathf.Sin(cameraParent.eulerAngles.y / 180 * Mathf.PI)
                        + horizontalSpeed * Mathf.Cos(cameraParent.eulerAngles.y / 180 * Mathf.PI); //x�����̈ړ����x
        float zSpeed = verticalSpeed * Mathf.Cos(cameraParent.eulerAngles.y / 180 * Mathf.PI)
                        + horizontalSpeed * Mathf.Sin(-1 * cameraParent.eulerAngles.y / 180 * Mathf.PI);�@//z�����̈ړ����x

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
