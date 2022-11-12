using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnotherController : MonoBehaviour
{
    [Header("自キャラ")] public GameObject myCharacter;
    [Header("移動速度")] public float speed;
    [Header("カメラ回転速度")] public float cameraSpeed;

    private Rigidbody rb;

    void Start()
    {
        rb = myCharacter.GetComponent<Rigidbody>();
    }

    void Update()
    {
        Movement();
    }

    void Movement()
    {
        rb.velocity = new Vector3(0, 0, 0);
    }
}
