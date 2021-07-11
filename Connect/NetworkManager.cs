using UnityEngine;
using System.Net.Sockets;
using System;
using System.Text;

public class NetworkManager : MonoBehaviour
{
    void Start()
    {
        TcpClient client = new TcpClient("127.0.0.1", 8080);

        string msg = "Hello Unity!";
        byte[] buff = Encoding.ASCII.GetBytes(msg);

        NetworkStream stream = client.GetStream();
        stream.Write(buff, 0, buff.Length);

        byte[] outbuf = new byte[1024];
        int nbytes = stream.Read(outbuf, 0, outbuf.Length);
        string output = Encoding.ASCII.GetString(outbuf, 0, nbytes);

        stream.Close();
        client.Close();
    }

    void Update()
    {
        
    }
}