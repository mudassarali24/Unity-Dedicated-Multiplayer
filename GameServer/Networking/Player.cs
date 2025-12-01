using System.Net.Sockets;

namespace GameServer.Networking
{
    public class Player
    {
        public int Id { get; private set; }
        public TcpClient client { get; private set; }
        public float x;
        public float y;
        public float z;
        public Player(int id, TcpClient _client, float x, float y, float z)
        {
            Id = id;
            client = _client;
            this.x = x;
            this.y = y;
            this.z = z;
        }
    }
}