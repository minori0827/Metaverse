using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("移動速度")] public float speed;
    [Header("カメラ速度")] public float cameraSpeed;
    [Header("カメラ")] public Transform cameraParent;

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

        if (Input.GetKey(KeyCode.W)) //奥移動
        {
            zDirection = 1;
        }
        else if (Input.GetKey(KeyCode.S)) //手前移動
        {
            zDirection = -1;
        }

        if (Input.GetKey(KeyCode.D)) //右移動
        {
            xDirection = 1;
        }
        else if (Input.GetKey(KeyCode.A)) //左移動
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
        if (Input.GetKey(KeyCode.E)) //カメラ右移動
        {
            eulerAngles.y -= cameraSpeed;
        }
        else if (Input.GetKey(KeyCode.Q)) //カメラ左移動
        {
            eulerAngles.y += cameraSpeed;
        }
        cameraParent.eulerAngles = eulerAngles;
    }
}
