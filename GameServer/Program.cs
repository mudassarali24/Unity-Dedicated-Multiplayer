using System;
using GameServer.Simulation;
using GameServer.Networking;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Game Server Booting...");

        var tcpServer = new TcpServer();
        tcpServer.Start(7777);
        Console.WriteLine("TCP Server Started!");
        ServerReference.Instance.TcpServer = tcpServer;

        // Start Game Loop in a separate thread
        var gameLoop = new GameLoop();
        Thread gameLoopThread = new Thread(gameLoop.Start);
        gameLoopThread.Start();

        Console.WriteLine("Server is Running...");

        // Keep the main thread alive
        Thread.Sleep(Timeout.Infinite);
    }
}