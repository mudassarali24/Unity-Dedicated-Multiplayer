using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Concurrent;
using GameServer.Simulation;
using GameServer.Utils;

namespace GameServer.Networking
{
    public class TcpServer
    {
        private TcpListener listener;
        private Thread listenerThread;
        public ConcurrentDictionary<int, Player> players = new ConcurrentDictionary<int, Player>();
        public ConcurrentDictionary<int, Enemy> enemies = new ConcurrentDictionary<int, Enemy>();

        private const int INIT_ENEMIES_COUNT = 5;
        private int playerCounter = 0;
        private int enemyCounter = 0;
        private Random rng = new Random();

        public void Start(int port)
        {
            listener = new TcpListener(IPAddress.Any, port);
            listener.Start();

            Console.WriteLine($"(TCP) Listening on Port: {port}");

            listenerThread = new Thread(ListenForClients);
            listenerThread.Start();

            SpawnEnemies();
        }
        private void ListenForClients()
        {
            while (true)
            {
                var client = listener.AcceptTcpClient();
                int id = ++playerCounter;
                var spawnPos = GetRandomSpawnPos();
                Vector3 pos = new Vector3(spawnPos.x, spawnPos.y, spawnPos.z);
                Vector3 rot = new Vector3(0, 0, 0);
                var data = new Player(id, client, pos, rot);

                players.TryAdd(id, data);
                Console.WriteLine($"(TCP) Player {id} connected!");
                AssignIDToPlayer(id);
                BroadcastSpawn(id, pos, rot);
                // Send existing players to this new connected player
                SendExistingPlayersTo(id);
                // Send existing enemies to this new connected player
                SendExistingEnemiesTo(id);

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
                    // Console.WriteLine($"(TCP) Player {id}: {message.Trim()}");

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
                case "ANIM":
                    HandleAnimation(id, parts);
                    break;
                case "SHOOT":
                    HandleShoot(id, parts);
                    break;
                case "ENEMY_POS":
                    HandleEnemyPos(id, parts);
                    break;
                case "ENEMY_ROT":
                    HandleEnemyRot(id, parts);
                    break;
                case "ENEMY_TARGET_PLAYER":
                    HandleEnemyTargetPID(id, parts);
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
        public void BroadcastSpawn(int id, Vector3 pos, Vector3 rot)
        {
            string msg = $"SPAWN:{id}:{pos.x}:{pos.y}:{pos.z}:{rot.x}:{rot.y}:{rot.z}";
            Broadcast(msg);
        }
        #endregion



        #region MESSAGE_HANDLERS

        private void HandlePosition(int id, string[] parts)
        {
            if (parts[0] != "POS") return;

            Vector3 pos = new Vector3();

            // Example: POS:12.4:-5.6:3.2
            if (float.TryParse(parts[1], out pos.x) &&
                float.TryParse(parts[2], out pos.y) &&
                float.TryParse(parts[3], out pos.z))
            {
                if (players.TryGetValue(id, out Player player))
                {
                    player.pos = pos;
                }
            }
        }

        private void HandleRotation(int id, string[] parts)
        {
            if (parts[0] != "ROT") return;

            Vector3 rot = new Vector3();
            // Example: ROT:12.4:-5.6:3.2
            if (float.TryParse(parts[1], out rot.x) &&
                float.TryParse(parts[2], out rot.y) &&
                float.TryParse(parts[3], out rot.z))
            {
                if (players.TryGetValue(id, out Player player))
                {
                    player.rot = rot;
                }
            }
        }
        private void HandleAnimation(int id, string[] parts)
        {
            if (parts[0] != "ANIM") return;

            // Example: ANIM:State:2.0
            if (!string.IsNullOrEmpty(parts[1]) &&
                float.TryParse(parts[2], out float val))
            {
                string param = parts[1];
                if (players.TryGetValue(id, out Player player))
                {
                    player.animations[param] = val;
                }
            }
        }

        public void HandleShoot(int id, string[] parts)
        {
            if (parts[0] != "SHOOT") return;

            Vector3 shootPoint = new Vector3();
            Quaternion shootPtRot = new Quaternion();
            Vector3 hitPoint = new Vector3();

            // Example: SHOOT:X,Y,Z:X,Y,Z,W:X,Y,Z:-1
            string[] shootPointParts = parts[1].Split(',');
            string[] shootPtRotParts = parts[2].Split(',');
            string[] hitPointParts = parts[3].Split(',');

            if (float.TryParse(shootPointParts[0], out shootPoint.x) // Shoot Point Pos
                && float.TryParse(shootPointParts[1], out shootPoint.y)
                && float.TryParse(shootPointParts[2], out shootPoint.z)
                && float.TryParse(shootPtRotParts[0], out shootPtRot.x) // Shoot Point Rot
                && float.TryParse(shootPtRotParts[1], out shootPtRot.y)
                && float.TryParse(shootPtRotParts[2], out shootPtRot.z)
                && float.TryParse(shootPtRotParts[3], out shootPtRot.w)
                && float.TryParse(hitPointParts[0], out hitPoint.x) // Hit Point pos
                && float.TryParse(hitPointParts[1], out hitPoint.y)
                && float.TryParse(hitPointParts[2], out hitPoint.z)
                && int.TryParse(parts[4], out int targetId))
            {
                string msg = $"UPD_SHOOT:{id}:{shootPoint.x},{shootPoint.y},{shootPoint.z}:{shootPtRot.x},{shootPtRot.y},{shootPtRot.z},{shootPtRot.w}:{hitPoint.x},{hitPoint.y},{hitPoint.z}:{targetId}";
                Broadcast(msg);
            }
        }
        private void HandleEnemyPos(int id, string[] parts)
        {
            if (parts[0] != "ENEMY_POS") return;
            Vector3 pos = new Vector3();
            if (float.TryParse(parts[1], out pos.x)
                && float.TryParse(parts[2], out pos.y)
                && float.TryParse(parts[3], out pos.z))
            {
                if (enemies.TryGetValue(id, out Enemy enemy))
                {
                    enemy.currentPos = pos;
                }
            }
        }
        private void HandleEnemyRot(int id, string[] parts)
        {
            if (parts[0] != "ENEMY_ROT") return;
            if (float.TryParse(parts[1], out float rotY))
            {
                if (enemies.TryGetValue(id, out Enemy enemy))
                {
                    enemy.currentRotY = rotY;
                }
            }
        }
        private void HandleEnemyTargetPID(int id, string[] parts)
        {
            if (parts[0] != "ENEMY_TARGET_PLAYER") return;
            if (int.TryParse(parts[1], out int enemyID)
                && int.TryParse(parts[2], out int pID))
            {
                if (enemies.TryGetValue(enemyID, out Enemy enemy))
                {
                    Console.WriteLine($"Updating target id of enemy {enemyID} to {pID}!");
                    enemy.targetPlayerId = pID;
                }
            }
        }

        #endregion

        #region UTILS

        private void SpawnEnemies()
        {
            for (int i = 0; i < INIT_ENEMIES_COUNT; i++)
            {
                int id = ++enemyCounter;
                var spawnPos = GetRandomSpawnPosEnemy();
                Vector3 pos = new Vector3(spawnPos.x, spawnPos.y, spawnPos.z);
                var data = new Enemy(id, pos, 0);

                enemies.TryAdd(id, data);
                Console.WriteLine($"(TCP) Enemy {id} spawned!");

                SendEnemySpawned(data);
            }
        }

        private void SendEnemySpawned(Enemy enemy)
        {
            // Example: SPAWN_ENEMY:1:x,y,z:2:3:CHASING
            string msg = $"SPAWN_ENEMY:{enemy.enemyId}:{enemy.currentPos.x},{enemy.currentPos.y},{enemy.currentPos.z}:{enemy.currentRotY}:{enemy.targetPlayerId}:{enemy.currentState.ToString()}";
            Broadcast(msg);
        }

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
        private (float x, float y, float z) GetRandomSpawnPosEnemy()
        {
            float x = (float)(rng.NextDouble() * 40 - 30);
            float z = (float)(rng.NextDouble() * 40 - 30);
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
        private void SendExistingEnemiesTo(int newPID)
        {
            foreach (var enemy in enemies.Values)
            {
                // Example: SPAWN_ENEMY:1:x,y,z:2:3:CHASING
                string msg = $"SPAWN_ENEMY:{enemy.enemyId}:{enemy.currentPos.x},{enemy.currentPos.y},{enemy.currentPos.z}:{enemy.currentRotY}:{enemy.targetPlayerId}:{enemy.currentState.ToString()}";
                BroadcastToClient(newPID, msg);
            }
        }

        #endregion
    }
}