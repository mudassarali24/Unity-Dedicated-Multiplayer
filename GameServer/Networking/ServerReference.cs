using GameServer.Networking;

public class ServerReference
{
    public static ServerReference Instance { get; private set; } = new ServerReference();

    public TcpServer TcpServer { get; set; }
}