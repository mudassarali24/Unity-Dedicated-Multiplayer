using System.Net.Sockets;
using GameServer.Utils;

namespace GameServer.Networking
{
    public class Player
    {
        public int Id { get; private set; }
        public TcpClient client { get; private set; }
        public Vector3 pos;
        public Vector3 rot;
        public Dictionary<string, float> animations;
        public Player(int id, TcpClient _client, Vector3 pos, Vector3 rot)
        {
            Id = id;
            client = _client;
            this.pos = pos;
            this.rot = rot;
            animations = new Dictionary<string, float>();
        }
    }
}