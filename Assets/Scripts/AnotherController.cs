using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnotherController : MonoBehaviour
{
    [Header("���L����")] public GameObject myCharacter;
    [Header("�ړ����x")] public float speed;
    [Header("�J������]���x")] public float cameraSpeed;

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
