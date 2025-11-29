using System;
using System.Threading;
using GameServer.Networking;

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
            
            // Broadcast all player positions
            foreach (var kv in TcpServer.players)
            {
                var player = kv.Value;
                string msg = $"UPD:{player.x}:{player.y}:{player.z}";
                TcpServer.Broadcast(msg);
            }
        }

        public void Stop() => isRunning = false;
    }
}