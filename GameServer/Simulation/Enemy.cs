using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameServer.Networking;
using GameServer.Utils;

namespace GameServer.Simulation
{
    public enum EnemyState
    {
        WANDERING, CHASING, ATTACKING
    }
    public class Enemy
    {
        public int enemyId;
        public Vector3 currentPos;
        public float currentRotY;
        public EnemyState currentState;
        public int targetPlayerId;

        private const float CHASE_DISTANCE = 8.0f;
        private const float ATTACK_DISTANCE = 6.0f;

        public Enemy(int enemyId, Vector3 pos, float rot)
        {
            this.enemyId = enemyId;
            currentPos = pos;
            currentRotY = rot;
            currentState = EnemyState.WANDERING;
            targetPlayerId = -1;
        }

        public void UpdateState()
        {
            // if no player is target, enemy will wander only
            if (targetPlayerId == -1)
            {
                currentState = EnemyState.WANDERING;
                return;
            }

            ServerReference.Instance.TcpServer.players.TryGetValue(targetPlayerId, out Player targetPlayer);

            if (targetPlayer == null)
            {
                currentState = EnemyState.WANDERING;
                return;
            }

            Vector3 targetPlayerPos = ServerReference.Instance.TcpServer.players[targetPlayerId].pos;

            // if (Vector3.Distance(targetPlayerPos, currentPos) <= CHASE_DISTANCE)
            // {
            //     currentState = EnemyState.CHASING;
            // }
            if (Vector3.Distance(targetPlayerPos, currentPos) <= ATTACK_DISTANCE)
            {
                currentState = EnemyState.ATTACKING;
            }
            else
            {
                currentState = EnemyState.CHASING;
            }
        }
    }
}