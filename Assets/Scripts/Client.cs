using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// TCP �ʐM���s���N���C�A���g���̃R���|�[�l���g
/// </summary>
public class Client : MonoBehaviour
{
    //================================================================================
    // �ϐ�
    //================================================================================
    // ���� IP �A�h���X�ƃ|�[�g�ԍ��̓T�[�o���Ɠ��ꂷ�邱��
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

    private string m_message = "�f�t�H���g"; // �T�[�o�ɑ��M���镶����
    private string m_message2 = string.Empty; //�T�[�o����󂯎�镶����
    private string myPosition = string.Empty; //�T�[�o����󂯎��ʒu���

    private float anotherX;
    private float anotherY;
    private float anotherZ;

    //================================================================================
    // �֐�
    //================================================================================
    /// <summary>
    /// ���������鎞�ɌĂяo����܂�
    /// </summary>
    private void Awake()
    {
        try
        {
            // �w�肳�ꂽ IP �A�h���X�ƃ|�[�g�ŃT�[�o�ɐڑ����܂�
            m_tcpClient = new TcpClient(s_ipAddress, s_port);
            m_networkStream = m_tcpClient.GetStream();
            m_isConnection = true;

            Debug.LogFormat("�ڑ�����");
        }
        catch (SocketException)
        {
            // �T�[�o���N�����Ă��炸�ڑ��Ɏ��s�����ꍇ�͂����ɗ��܂�
            Debug.LogError("�ڑ����s");
        }

        myCharacter = playerController.myCharacter;
        anotherCharacter = anotherController.myCharacter;
        // �N���C�A���g���當�������M���鏈����񓯊��Ŏ��s���܂�
        // �񓯊��Ŏ��s���Ȃ��Ɛڑ����I������܂Ŏ�M����������� UI �ɕ\���ł��܂���
        Task.Run(() => OnProcess());
    }

    private void OnProcess()
    {
        var ipAddress = IPAddress.Parse(c_ipAddress);
        m_tcpListener = new TcpListener(ipAddress, c_port);
        m_tcpListener.Start();

        Debug.Log("�ҋ@��");

        // �N���C�A���g����̐ڑ���ҋ@���܂�
        m_tcpClient = m_tcpListener.AcceptTcpClient();

        Debug.Log("�ڑ�����");

        // �N���C�A���g����̐ڑ������������̂�
        // �N���C�A���g���當���񂪑��M�����̂�ҋ@���܂�
        m_networkStream = m_tcpClient.GetStream();

        while (true)
        {
            var buffer = new byte[256];
            var count = m_networkStream.Read(buffer, 0, buffer.Length);

            // �N���C�A���g����̐ڑ����ؒf���ꂽ�ꍇ��
            if (count == 0)
            {
                Debug.Log("�ؒf");

                // �ʐM�Ɏg�p�����C���X�^���X��j������
                OnDestroy();

                // �ēx�N���C�A���g����̐ڑ���ҋ@���܂�
                Task.Run(() => OnProcess());

                break;
            }

            // �N���C�A���g���當�������M�����ꍇ��
            // GUI �ƃ��O�ɏo�͂��܂�
            int id = Data.IdentifyType(buffer);

            if (id == 1)
            {
                var message = Data.ByteToString(buffer, count);
                m_message2 += message + "\n";
                Debug.LogFormat("��M�����F{0}", message);
            }
            else if (id == 2)
            {
                float[] position = Data.ByteToFloat(buffer);
                var x = position[0];
                var y = position[1];
                var z = position[2];
                Debug.LogFormat("��M�����F{0},{1},{2}", x, y, z);
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
                // �T�[�o�Ɉʒu���𑗐M���܂�
                var x = myCharacter.transform.position.x;
                var y = myCharacter.transform.position.y;
                var z = myCharacter.transform.position.z;
                var writeBuffer = Data.FloatToByte(myNumber, x, y, z);
                m_networkStream.Write(writeBuffer, 0, writeBuffer.Length);

                Debug.LogFormat("���M�����F{0},{1},{2}", x, y, z);

                /*byte[] readBuffer = new byte[12];
                m_networkStream.Read(readBuffer, 0, readBuffer.Length);
                anotherCharacter.transform.position =
                    new Vector3(BitConverter.ToSingle(readBuffer, 0), BitConverter.ToSingle(readBuffer, 4), BitConverter.ToSingle(readBuffer, 8));
                */
                anotherCharacter.transform.position = new Vector3(anotherX, anotherY, anotherZ);
            }
            catch (Exception)
            {
                // �T�[�o���N�����Ă��炸���M�Ɏ��s�����ꍇ�͂����ɗ��܂�
                // SocketException �^���Ɨ�O�̃L���b�`���ł��Ȃ��悤�Ȃ̂�
                // Exception �^�ŗ�O���L���b�`���Ă��܂�
                Debug.LogError("���M���s");
            }

            timeElapsed = 0.0f;
        }
    }

    /// <summary>
    /// GUI ��`�悷�鎞�ɌĂяo����܂�
    /// </summary>
    public void OnGUI()
    {
        // Awake �֐��Őڑ��Ɏ��s�����ꍇ�͂��̎|��\�����܂�
        if (!m_isConnection)
        {
            GUILayout.Label("�ڑ����Ă��܂���");
            return;
        }

        // �T�[�o�ɑ��M���镶����
        m_message = GUILayout.TextField(m_message);

        // ���M�{�^���������ꂽ��
        if (GUILayout.Button("���M"))
        {
            try
            {
                // �T�[�o�ɕ�����𑗐M���܂�
                var buffer = Data.StringToByte(myNumber, m_message);
                m_networkStream.Write(buffer, 0, buffer.Length);

                Debug.LogFormat("���M�����F{0}", m_message);
            }
            catch (Exception)
            {
                // �T�[�o���N�����Ă��炸���M�Ɏ��s�����ꍇ�͂����ɗ��܂�
                // SocketException �^���Ɨ�O�̃L���b�`���ł��Ȃ��悤�Ȃ̂�
                // Exception �^�ŗ�O���L���b�`���Ă��܂�
                Debug.LogError("���M���s");
            }
        }

        GUILayout.TextArea(myPosition);
        GUILayout.TextArea(m_message2);
    }

    /// <summary>
    /// �j�����鎞�ɌĂяo����܂�
    /// </summary>
    private void OnDestroy()
    {
        // �ʐM�Ɏg�p�����C���X�^���X��j�����܂�
        // Awake �֐��ŃC���X�^���X�̐����Ɏ��s���Ă���\��������̂�
        // null �������Z�q���g�p���Ă��܂�
        m_tcpClient?.Dispose();
        m_networkStream?.Dispose();

        Debug.Log("�ؒf");
    }
}
