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
    }

    void Update()
    {

    }

    public void Movement(float x, float y, float z)
    {
        myCharacter.transform.position = new Vector3(x, y, z);
    }
}
