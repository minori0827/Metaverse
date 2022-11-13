using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("自キャラ")] public GameObject myCharacter;
    [Header("移動速度")] public float speed;
    [Header("ジャンプ速度")] public float jumpSpeed;
    [Header("カメラ回転速度")] public float cameraSpeed;
    [Header("接地判定")] public GroundCheck ground;

    private Transform cameraParent; //カメラ位置
    private Transform body; //プレイヤーのオブジェクト
    private Rigidbody rb; //プレイヤーの当たり判定
    private float xSpeed = 0; //x軸方向の移動速度
    private float ySpeed = 0; //y軸方向の移動速度
    private float zSpeed = 0; //z軸方向の移動速度
    private float angleMovableTime = 0.05f; //プレイヤーの向きを変えるのに必要な時間
    private bool isGround = false; //接地判定
    private bool isJump = false; //ジャンプ中かどうか
    private float jumpTime = 0; //ジャンプしてからの時間
    private float jumpableTime = 2; //元の落下速度に戻るまでの時間


    void Start()
    {
        rb = myCharacter.GetComponent<Rigidbody>();
        cameraParent = myCharacter.transform.Find("CameraParent");
        body = myCharacter.transform.Find("Body");
        ySpeed = -jumpSpeed;
    }

    void FixedUpdate()
    {
        Movement();
        CameraMove();
    }

    void Movement()
    {
        isGround = ground.IsGround();

        float horizontalDirection = 0; //前後の移動方法
        float verticalDirection = 0; //左右の移動方法
        float horizontalSpeed = 0; //前後の移動速度
        float verticalSpeed = 0; //左右の移動速度

        if (Input.GetKey(KeyCode.W)) //奥移動
        {
            verticalDirection = 1;
        }
        else if (Input.GetKey(KeyCode.S)) //手前移動
        {
            verticalDirection = -1;
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

        float xSpeedBuffer = verticalSpeed * Mathf.Sin(cameraParent.eulerAngles.y / 180 * Mathf.PI)
                        + horizontalSpeed * Mathf.Cos(cameraParent.eulerAngles.y / 180 * Mathf.PI); //x軸方向の移動速度仮置き
        float zSpeedBuffer = verticalSpeed * Mathf.Cos(cameraParent.eulerAngles.y / 180 * Mathf.PI)
                        + horizontalSpeed * Mathf.Sin(-1 * cameraParent.eulerAngles.y / 180 * Mathf.PI); //z軸方向の移動速度仮置き

        if (xSpeedBuffer != xSpeed || zSpeedBuffer != zSpeed)
        {
            Invoke("AngleMove", angleMovableTime);
        }

        if (horizontalDirection == 0 && verticalDirection == 0)
        {
            CancelInvoke("AngleMove");
        }

        xSpeed = xSpeedBuffer;
        zSpeed = zSpeedBuffer;


        if (isGround)
        {
            jumpTime = 0;
            ySpeed = -jumpSpeed;
            isJump = false;
            if (Input.GetKey(KeyCode.Space)) //ジャンプ
            {
                isJump = true;
            }
        }

        if (isJump)
        {
            jumpTime += Time.deltaTime;
            if (jumpableTime > jumpTime)
            {
                ySpeed = jumpSpeed * Mathf.Cos(jumpTime / jumpableTime * Mathf.PI);
            }
            else
            {
                ySpeed = -jumpSpeed;
            }
        }

        rb.velocity = new Vector3(xSpeed, ySpeed, zSpeed); //移動処理
    }
    void AngleMove()
    {
        Vector3 eulerAngles = body.eulerAngles;
        eulerAngles.y = Mathf.Atan2(xSpeed, zSpeed) * Mathf.Rad2Deg;
        body.eulerAngles = eulerAngles; //プレイヤーの向きの処理
    }

    void CameraMove()
    {
        Vector3 eulerAngles = cameraParent.eulerAngles;
        if (Input.GetKey(KeyCode.E)) //カメラ右回転
        {
            eulerAngles.y -= cameraSpeed;
        }
        else if (Input.GetKey(KeyCode.Q)) //カメラ左回転
        {
            eulerAngles.y += cameraSpeed;
        }
        cameraParent.eulerAngles = eulerAngles;
    }
}

