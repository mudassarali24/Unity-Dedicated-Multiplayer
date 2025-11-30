using UnityEngine;
using System;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using PimDeWitte.UnityMainThreadDispatcher;

public class TcpClientManager : MonoBehaviour
{
    private TcpClient client;
    private NetworkStream stream;
    private Thread receiveThread;
    public static TcpClientManager Instance;

    void Awake()
    {
        Instance = this;
    }

    public void Connect(string ip, int port)
    {
        try
        {
            client = new TcpClient(ip, port);
            stream = client.GetStream();

            receiveThread = new Thread(ReceiveLoop);
            receiveThread.Start();
            Debug.Log($"Connected to Server!");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Connection Error: {ex}");
        }
    }

    private void ReceiveLoop()
    {
        byte[] buffer = new byte[1024];

        while (true)
        {
            int bytes = stream.Read(buffer, 0, buffer.Length);
            if (bytes <= 0) continue; // continue the loop but skip this one

            string message = Encoding.UTF8.GetString(buffer, 0, bytes);

            foreach (string line in message.Split('\n'))
            {   
                if (!string.IsNullOrEmpty(line))
                    ProcessMessage(line.Trim());
            }
        }
    }

    private void ProcessMessage(string msg)
    {
        // Handle Message on game thread
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
           GameClientMessageHandler.Handle(msg); 
        });
    }

    public void Send(string msg)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(msg + "\n");
        stream.Write(bytes, 0, bytes.Length);
    }
}