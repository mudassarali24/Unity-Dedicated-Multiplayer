using UnityEngine;
using System;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using PimDeWitte.UnityMainThreadDispatcher;
using System.IO;
using System.Net;

public class TcpClientManager : MonoBehaviour
{
    private TcpClient client;
    private NetworkStream stream;
    private Thread receiveThread;
    private volatile bool isRunning = false;
    public static TcpClientManager Instance;

    void Awake()
    {
        Instance = this;
    }

    void OnDestroy()
    {
        Disconnect();
    }

    public void Connect(string ip, int port)
    {
        try
        {
            ip = ip.Trim();

            IPAddress address = IPAddress.Parse(ip);

            client = new TcpClient();
            client.Connect(address, port);
            stream = client.GetStream();

            isRunning = true;

            receiveThread = new Thread(ReceiveLoop);
            receiveThread.IsBackground = true;
            receiveThread.Start();

            Debug.Log($"Connected to Server!");
        }
        catch (SocketException se)
        {
            Debug.LogError($"Socket Exception: {se.Message}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Connection Error: {ex}");
        }
    }
    private void ReceiveLoop()
    {
        byte[] buffer = new byte[1024];

        while (isRunning)
        {
            if (!stream.CanRead) break;

            int bytes = 0;
            try
            {
                bytes = stream.Read(buffer, 0, buffer.Length);
            }
            catch (IOException)
            {
                // Stream closed or connection lost
                break;
            }
            catch (ObjectDisposedException)
            {
                break;
            }

            if (bytes <= 0) break; // connection closed by the server

            string message = Encoding.UTF8.GetString(buffer, 0, bytes);

            foreach (string line in message.Split('\n'))
            {
                if (!string.IsNullOrEmpty(line))
                    ProcessMessage(line.Trim());
            }
        }

        Debug.Log("Receive loop ended.");
    }

    private void ProcessMessage(string msg)
    {
        // Handle Message on game thread
        if (!UnityMainThreadDispatcher.Exists()) return;
        // Debug.Log($"Message Received: {msg}");
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

    public void Disconnect()
    {
        isRunning = false;
        if (stream != null)
            stream.Close();

        if (client != null)
            client.Close();

        if (receiveThread != null && receiveThread.IsAlive)
            receiveThread.Join(); // Wait for thread to finish safely

        Debug.Log("Disconnected from server.");
    }
}