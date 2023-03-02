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
    }

    void Update()
    {

    }

    public void Movement(float x, float y, float z)
    {
        myCharacter.transform.position = new Vector3(x, y, z);
    }
}
