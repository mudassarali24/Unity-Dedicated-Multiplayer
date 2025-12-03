using System;
using System.Threading;
using GameServer.Networking;
using GameServer.Utils;

namespace GameServer.Simulation
{
    public class GameLoop
    {
        private const int TICK_RATE = 60;
        private const int TICK_MS = 1000 / TICK_RATE;
        private bool isRunning = false;

        private TcpServer TcpServer => ServerReference.Instance.TcpServer;

        public void Start()
        {
            isRunning = true;
            Console.WriteLine("Game Loop Started!");

            while (isRunning)
            {
                var start = DateTime.UtcNow;

                Tick(); // TODO: Update world state

                var diff = (int)(DateTime.UtcNow - start).Milliseconds;
                if (diff < TICK_MS)
                    Thread.Sleep(TICK_MS - diff); // sleep to match MS
            }
        }

        private void Tick()
        {
            // Game logic here

            BroadcastPositions();
            BroadcastAnimations();
        }

        public void Stop() => isRunning = false;

        #region BROADCASTING

        /// <summary>
        /// Broadcast all player positions
        /// </summary>
        private void BroadcastPositions()
        {
            foreach (var kv in TcpServer.players)
            {
                var player = kv.Value;
                string msg = $"UPD_POS:{player.Id}:{player.pos.x}:{player.pos.y}:{player.pos.z}:{player.rot.x}:{player.rot.y}:{player.rot.z}";
                TcpServer.Broadcast(msg);
            }
        }
        /// <summary>
        /// Broadcast all player positions
        /// </summary>
        private void BroadcastAnimations()
        {
            foreach (var kv in TcpServer.players)
            {
                var player = kv.Value;
                foreach (var akv in player.animations)
                {
                    string msg = $"UPD_ANIM:{player.Id}:{akv.Key}:{akv.Value}";
                    TcpServer.Broadcast(msg);
                }
            }
        }

        #endregion
    }
}