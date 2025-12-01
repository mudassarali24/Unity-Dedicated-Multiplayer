using System.Net.Sockets;

namespace GameServer.Networking
{
    public struct Position
    {
        public float x;
        public float y;
        public float z;      
        public Position(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
    }
    public struct Rotation
    {
        public float x;
        public float y;
        public float z;

        public Rotation(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
    }
    public class Player
    {
        public int Id { get; private set; }
        public TcpClient client { get; private set; }
        public Position pos;
        public Rotation rot;
        public Player(int id, TcpClient _client, Position pos, Rotation rot)
        {
            Id = id;
            client = _client;
            this.pos = pos;
            this.rot = rot;
        }
    }
}