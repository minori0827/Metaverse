using System;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public static class Data
{
    // 1:ï∂éöóÒ 2:à íuèÓïÒ

    public static byte[] StringToByte(int myNumber, string m_message)
    {
        var str = Encoding.UTF8.GetBytes(m_message);
        byte[] id = new byte[1];
        id[0] = 1;
        byte[] number = BitConverter.GetBytes(myNumber);
        var buffer = MargeByte(MargeByte(id,number), str);
        return buffer;
    }

    public static byte[] FloatToByte(int myNumber, float x, float y, float z)
    {
        byte[] floX = BitConverter.GetBytes(x);
        byte[] floY = BitConverter.GetBytes(y);
        byte[] floZ = BitConverter.GetBytes(z);
        byte[] id = new byte[1];
        id[0] = 2;
        byte[] number = BitConverter.GetBytes(myNumber);
        var buffer = MargeByte(MargeByte(MargeByte(MargeByte(id,number), floX), floY), floZ);
        return buffer;
    }

    public static int IdentifyType(byte[] buffer)
    {
        if (buffer[0] == 1) return 1;
        else if (buffer[0] == 2) return 2;
        else return 0;
    }

    public static int IdentifyPlayer(byte[] buffer)
    {
        return BitConverter.ToInt32(buffer, 1);
    }

    public static string ByteToString(byte[] buffer, int count)
    {
        var message = Encoding.UTF8.GetString(buffer, 5, count);
        return message;
    }

    public static float[] ByteToFloat(byte[] buffer)
    {
        float[] position = new float[3];
        position[0] = BitConverter.ToSingle(buffer, 5);
        position[1] = BitConverter.ToSingle(buffer, 9);
        position[2] = BitConverter.ToSingle(buffer, 13);
        return position;
    }

    public static byte[] MargeByte(byte[] baseByte, byte[] addByte)
    {
        byte[] b = new byte[baseByte.Length + addByte.Length];
        for (int i = 0; i < b.Length; i++)
        {
            if (i < baseByte.Length) b[i] = baseByte[i];
            else b[i] = addByte[i - baseByte.Length];
        }
        return b;
    }
}