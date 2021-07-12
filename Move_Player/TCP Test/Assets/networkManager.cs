using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class networkManager : MonoBehaviour
{
    Thread mThread;
    public string connectionIP = "127.0.0.1";
    public int connectionPort = 25001;
    IPAddress localAdd;
    TcpListener listener;
    TcpClient client;
    bool running;

    // Position is the data being received in this example
    Vector3 position = Vector3.zero;

    private void Update()
    {
        // Set this object's position in the scene according to the position received
        transform.position = position;
    }

    private void Start()
    {
        // Receive on a separate thread so Unity doesn't freeze waiting for data
        ThreadStart ts = new ThreadStart(GetInfo);
        mThread = new Thread(ts);
        mThread.Start();
    }

    public static string GetLocalIPAddress()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                return ip.ToString();
            }
        }
        throw new System.Exception("No network adapters with an IPv4 address in the system!");
    }

    void GetInfo()
    {
        localAdd = IPAddress.Parse(connectionIP);
        listener = new TcpListener(IPAddress.Any, connectionPort);
        listener.Start();

        client = listener.AcceptTcpClient();


        running = true;
        while (running)
        {
            Connection();
        }
        listener.Stop();
    }

    void Connection()
    {
        NetworkStream nwStream = client.GetStream();
        byte[] buffer = new byte[client.ReceiveBufferSize];

        int bytesRead = nwStream.Read(buffer, 0, client.ReceiveBufferSize);
        // Passing data as strings, not ideal but easy to use
        string dataReceived = Encoding.UTF8.GetString(buffer, 0, bytesRead);

        if (dataReceived != null)
        {
            if (dataReceived == "stop")
            {
                // Can send a string "stop" to kill the connection
                running = false;
            }
            else
            {
                // Convert the received string of data to the format we are using
                position += StringToVector3(dataReceived);
                print("moved");
                nwStream.Write(buffer, 0, bytesRead);
            }
        }
    }

    // Use-case specific function, need to re-write this to interpret whatever data is being sent
    public static Vector3 StringToVector3(string sKey)
    {
        Vector3 result = new Vector3(0f, 0f, 0f);
        // Remove the parentheses
        if (sKey.StartsWith("(") && sKey.EndsWith(")"))
        {
            sKey = sKey.Substring(1, sKey.Length - 2);
        }

        if (sKey == "w")
        {
            result.z = 1f;
        }
        else if (sKey == "s")
        {
            result.z = -1f;
        }
        else if (sKey == "a")
        {
            result.x = -1;
        }
        else if (sKey == "d")
        {
            result.x = 1f;
        }

        return result;
    }
}