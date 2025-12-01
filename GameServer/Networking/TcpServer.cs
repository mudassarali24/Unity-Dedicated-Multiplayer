using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Concurrent;
using GameServer.Simulation;

namespace GameServer.Networking
{
    public class TcpServer
    {
        private TcpListener listener;
        private Thread listenerThread;
        public ConcurrentDictionary<int, Player> players = new ConcurrentDictionary<int, Player>();
        private int playerCounter = 0;
        private Random rng = new Random();

        public void Start(int port)
        {
            listener = new TcpListener(IPAddress.Any, port);
            listener.Start();

            Console.WriteLine($"(TCP) Listening on Port: {port}");

            listenerThread = new Thread(ListenForClients);
            listenerThread.Start();
        }
        private void ListenForClients()
        {
            while (true)
            {
                var client = listener.AcceptTcpClient();
                int id = ++playerCounter;
                var spawnPos = GetRandomSpawnPos();
                Position pos = new Position(spawnPos.x, spawnPos.y, spawnPos.z);
                Rotation rot = new Rotation(0, 0, 0);
                var data = new Player(id, client, pos, rot);

                players.TryAdd(id, data);
                Console.WriteLine($"(TCP) Player {id} connected!");
                AssignIDToPlayer(id);
                BroadcastSpawn(id, pos, rot);
                // Send existing players to this new connected player
                SendExistingPlayersTo(id);

                Thread clientThread = new Thread(() => HandleClient(id, client));
                clientThread.Start();
            }
        }

        private void HandleClient(int id, TcpClient client)
        {
            var stream = client.GetStream(); // Get client stream
            byte[] buffer = new byte[1024];

            try
            {
                while (client.Connected)
                {
                    int bytes = stream.Read(buffer, 0, buffer.Length);
                    if (bytes <= 0) break; // stream disturbed

                    string message = System.Text.Encoding.UTF8.GetString(buffer, 0, bytes);
                    Console.WriteLine($"(TCP) Player {id}: {message.Trim()}");

                    HandleMessage(id, message);
                }
            }
            catch { }

            Disconnect(id);
        }

        private void HandleMessage(int id, string message)
        {
            string[] parts = message.Split(':');
            switch (parts[0])
            {
                case "POS":
                    HandlePosition(id, parts);
                    break;
                case "ROT":
                    HandleRotation(id, parts);
                    break;
            }
        }

        private void Disconnect(int id)
        {
            if (players.TryRemove(id, out var player))
            {
                player.client.Close();
                Console.WriteLine($"(TCP) Player {id} disconnected!");
            }
        }

        #region BROADCASTING
        /// <summary>
        /// Broadcast a message to all players
        /// </summary>
        /// <param name="message"></param>
        public void Broadcast(string message)
        {
            byte[] data = System.Text.Encoding.UTF8.GetBytes(message + "\n");
            foreach (var player in players)
            {
                try
                {
                    var stream = player.Value.client.GetStream();
                    if (stream.CanWrite)
                        stream.Write(data, 0, data.Length);
                }
                catch { }
            }
        }

        /// <summary>
        /// Broadcasts message to a specific client
        /// </summary>
        /// <param name="pID"></param>
        public void BroadcastToClient(int pID, string message)
        {
            if (players.TryGetValue(pID, out var player))
            {
                byte[] data = System.Text.Encoding.UTF8.GetBytes(message + "\n");
                try { player.client.GetStream().Write(data, 0, data.Length); }
                catch { }
            }
        }

        /// <summary>
        /// Broadcast spawn alert to everyone
        /// </summary>
        public void BroadcastSpawn(int id, Position pos, Rotation rot)
        {
            string msg = $"SPAWN:{id}:{pos.x}:{pos.y}:{pos.z}:{rot.x}:{rot.y}:{rot.z}";
            Broadcast(msg);
        }
        #endregion



        #region MESSAGE_HANDLERS

        private void HandlePosition(int id, string[] parts)
        {
            if (parts[0] != "POS") return;

            // Example: POS:12.4:-5.6:3.2
            if (parts.Length == 3 &&
                float.TryParse(parts[1], out float x) &&
                float.TryParse(parts[2], out float y) &&
                float.TryParse(parts[3], out float z))
            {
                if (players.TryGetValue(id, out Player player))
                {
                    player.pos.x = x;
                    player.pos.x = y;
                    player.pos.x = z;
                }
            }
        }

        private void HandleRotation(int id, string[] parts)
        {
            if (parts[0] != "ROT") return;

            // Example: ROT:12.4:-5.6:3.2
            if (parts.Length == 3 &&
                float.TryParse(parts[1], out float x) &&
                float.TryParse(parts[2], out float y) &&
                float.TryParse(parts[3], out float z))
            {
                if (players.TryGetValue(id, out Player player))
                {
                    player.rot.x = x;
                    player.rot.y = y;
                    player.rot.z = z;
                }
            }
        }

        #endregion

        #region UTILS

        private void AssignIDToPlayer(int newPID)
        {
            if (players.TryGetValue(newPID, out var player))
            {
                string msg = $"ASSIGN_ID:{newPID}";
                BroadcastToClient(newPID, msg);
            }
        }
        private (float x, float y, float z) GetRandomSpawnPos()
        {
            float x = (float)(rng.NextDouble() * 20 - 10);
            float z = (float)(rng.NextDouble() * 20 - 10);
            float y = 0f;

            return (x, y, z);
        }

        private void SendExistingPlayersTo(int newPID)
        {
            foreach (var player in players.Values)
            {
                string msg = $"SPAWN:{player.Id}:{player.pos.x}:{player.pos.y}:{player.pos.z}:{player.rot.x}:{player.rot.y}:{player.rot.z}";
                BroadcastToClient(newPID, msg);
            }
        }

        #endregion
    }
}