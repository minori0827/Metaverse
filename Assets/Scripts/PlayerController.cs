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

        float horizontalDirection = 0; //左右の移動方向
        float verticalDirection = 0; //前後の移動方向
        float horizontalSpeed = 0; //左右の移動速度
        float verticalSpeed = 0; //前後の移動速度

        if (Input.GetKey(KeyCode.W)) //奥移動
        {
            verticalDirection = 1;
        }
        else if (Input.GetKey(KeyCode.S)) //手前移動
        {
            verticalDirection  = -1;
        }

        if (Input.GetKey(KeyCode.D)) //右移動
        {
            horizontalDirection = 1;
        }
        else if (Input.GetKey(KeyCode.A)) //左移動
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
                        + horizontalSpeed * Mathf.Cos(cameraParent.eulerAngles.y / 180 * Mathf.PI); //x方向の移動速度
        float zSpeed = verticalSpeed * Mathf.Cos(cameraParent.eulerAngles.y / 180 * Mathf.PI)
                        + horizontalSpeed * Mathf.Sin(-1 * cameraParent.eulerAngles.y / 180 * Mathf.PI);　//z方向の移動速度

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
