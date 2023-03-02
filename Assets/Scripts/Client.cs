using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// TCP 通信を行うクライアント側のコンポーネント
/// </summary>
public class Client : MonoBehaviour
{
    //================================================================================
    // 変数
    //================================================================================
    // この IP アドレスとポート番号はサーバ側と統一すること
    public string c_ipAddress = "127.0.0.2";
    public int c_port = 2002;
    public string s_ipAddress = "127.0.0.1";
    public int s_port = 2001;
    public PlayerController playerController;
    public AnotherController anotherController;
    public int myNumber;

    private float timeOut = 1;
    private float timeElapsed;
    private TcpListener m_tcpListener;
    private TcpClient m_tcpClient;
    private NetworkStream m_networkStream;
    private bool m_isConnection;
    private GameObject myCharacter;
    private GameObject anotherCharacter;

    private string m_message = "デフォルト"; // サーバに送信する文字列
    private string m_message2 = string.Empty; //サーバから受け取る文字列
    private string myPosition = string.Empty; //サーバから受け取る位置情報

    private float anotherX;
    private float anotherY;
    private float anotherZ;

    //================================================================================
    // 関数
    //================================================================================
    /// <summary>
    /// 初期化する時に呼び出されます
    /// </summary>
    private void Awake()
    {
        try
        {
            // 指定された IP アドレスとポートでサーバに接続します
            m_tcpClient = new TcpClient(s_ipAddress, s_port);
            m_networkStream = m_tcpClient.GetStream();
            m_isConnection = true;

            Debug.LogFormat("接続成功");
        }
        catch (SocketException)
        {
            // サーバが起動しておらず接続に失敗した場合はここに来ます
            Debug.LogError("接続失敗");
        }

        myCharacter = playerController.myCharacter;
        anotherCharacter = anotherController.myCharacter;
        // クライアントから文字列を受信する処理を非同期で実行します
        // 非同期で実行しないと接続が終了するまで受信した文字列を UI に表示できません
        Task.Run(() => OnProcess());
    }

    private void OnProcess()
    {
        var ipAddress = IPAddress.Parse(c_ipAddress);
        m_tcpListener = new TcpListener(ipAddress, c_port);
        m_tcpListener.Start();

        Debug.Log("待機中");

        // クライアントからの接続を待機します
        m_tcpClient = m_tcpListener.AcceptTcpClient();

        Debug.Log("接続完了");

        // クライアントからの接続が完了したので
        // クライアントから文字列が送信されるのを待機します
        m_networkStream = m_tcpClient.GetStream();

        while (true)
        {
            var buffer = new byte[256];
            var count = m_networkStream.Read(buffer, 0, buffer.Length);

            // クライアントからの接続が切断された場合は
            if (count == 0)
            {
                Debug.Log("切断");

                // 通信に使用したインスタンスを破棄して
                OnDestroy();

                // 再度クライアントからの接続を待機します
                Task.Run(() => OnProcess());

                break;
            }

            // クライアントから文字列を受信した場合は
            // GUI とログに出力します
            int id = Data.IdentifyType(buffer);

            if (id == 1)
            {
                var message = Data.ByteToString(buffer, count);
                m_message2 += message + "\n";
                Debug.LogFormat("受信成功：{0}", message);
            }
            else if (id == 2)
            {
                float[] position = Data.ByteToFloat(buffer);
                var x = position[0];
                var y = position[1];
                var z = position[2];
                Debug.LogFormat("受信成功：{0},{1},{2}", x, y, z);
                var message = x.ToString() + "," + y.ToString() + "," + z.ToString();

                if (Data.IdentifyPlayer(buffer) == myNumber)
                {
                    myPosition = message;
                }
                else if (Data.IdentifyPlayer(buffer) != myNumber)
                {
                    anotherX = x;
                    anotherY = y;
                    anotherZ = z;
                }
            }
        }
    }

    private void Update()
    {
        timeElapsed += Time.deltaTime;

        if (timeElapsed >= timeOut)
        {
            try
            {
                // サーバに位置情報を送信します
                var x = myCharacter.transform.position.x;
                var y = myCharacter.transform.position.y;
                var z = myCharacter.transform.position.z;
                var writeBuffer = Data.FloatToByte(myNumber, x, y, z);
                m_networkStream.Write(writeBuffer, 0, writeBuffer.Length);

                Debug.LogFormat("送信成功：{0},{1},{2}", x, y, z);

                /*byte[] readBuffer = new byte[12];
                m_networkStream.Read(readBuffer, 0, readBuffer.Length);
                anotherCharacter.transform.position =
                    new Vector3(BitConverter.ToSingle(readBuffer, 0), BitConverter.ToSingle(readBuffer, 4), BitConverter.ToSingle(readBuffer, 8));
                */
                anotherCharacter.transform.position = new Vector3(anotherX, anotherY, anotherZ);
            }
            catch (Exception)
            {
                // サーバが起動しておらず送信に失敗した場合はここに来ます
                // SocketException 型だと例外のキャッチができないようなので
                // Exception 型で例外をキャッチしています
                Debug.LogError("送信失敗");
            }

            timeElapsed = 0.0f;
        }
    }

    /// <summary>
    /// GUI を描画する時に呼び出されます
    /// </summary>
    public void OnGUI()
    {
        // Awake 関数で接続に失敗した場合はその旨を表示します
        if (!m_isConnection)
        {
            GUILayout.Label("接続していません");
            return;
        }

        // サーバに送信する文字列
        m_message = GUILayout.TextField(m_message);

        // 送信ボタンが押されたら
        if (GUILayout.Button("送信"))
        {
            try
            {
                // サーバに文字列を送信します
                var buffer = Data.StringToByte(myNumber, m_message);
                m_networkStream.Write(buffer, 0, buffer.Length);

                Debug.LogFormat("送信成功：{0}", m_message);
            }
            catch (Exception)
            {
                // サーバが起動しておらず送信に失敗した場合はここに来ます
                // SocketException 型だと例外のキャッチができないようなので
                // Exception 型で例外をキャッチしています
                Debug.LogError("送信失敗");
            }
        }

        GUILayout.TextArea(myPosition);
        GUILayout.TextArea(m_message2);
    }

    /// <summary>
    /// 破棄する時に呼び出されます
    /// </summary>
    private void OnDestroy()
    {
        // 通信に使用したインスタンスを破棄します
        // Awake 関数でインスタンスの生成に失敗している可能性もあるので
        // null 条件演算子を使用しています
        m_tcpClient?.Dispose();
        m_networkStream?.Dispose();

        Debug.Log("切断");
    }
}
