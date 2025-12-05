using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyState
{
    WANDERING, CHASING, ATTACKING
}
public class NetworkEnemy : MonoBehaviour
{
    public int enemyId;
    public EnemyState currentState = EnemyState.WANDERING;
    public int targetPlayerID;
    [SerializeField] private float wanderRadius = 10f;
    [SerializeField] private float wanderInterval = 3f;

    private float wanderTimer;
    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        UpdatePosition(transform.position);
        UpdateRotation(transform.rotation.eulerAngles.y);
        ExecuteAI();
    }

    public void UpdatePosition(Vector3 position)
    {
        TcpClientManager.Instance.Send(
            $"ENEMY_POS:{position.x}:{position.y}:{position.z}");
    }
    public void UpdateRotation(float rotationY)
    {
        TcpClientManager.Instance.Send(
            $"ENEMY_ROT:{rotationY}");
    }
    public void UpdateTargetPlayer(int playerID)
    {
        Debug.Log($"Updating target id of enemy {enemyId} to target PID: {playerID}");
        TcpClientManager.Instance.Send(
            $"ENEMY_TARGET_PLAYER:{enemyId}:{playerID}");
    }

    private void ExecuteAI()
    {
        switch (currentState)
        {
            case EnemyState.WANDERING:
                Wandering();
                break;
            case EnemyState.CHASING:
                Chasing();
                break;
            case EnemyState.ATTACKING:
                Attacking();
                break;
        }
    }

    private void Wandering()
    {
        wanderTimer += Time.deltaTime;

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance || wanderTimer >= wanderInterval)
        {
            Vector3 newPos = RandomNavSphere(transform.position, wanderRadius);
            agent.SetDestination(newPos);
            wanderTimer = 0f;
        }
    }


    private Vector3 RandomNavSphere(Vector3 origin, float dist)
    {
        Vector3 randDir = Random.insideUnitSphere * dist;
        randDir += origin;

        NavMeshHit hit;
        NavMesh.SamplePosition(randDir, out hit, dist, NavMesh.AllAreas);

        return hit.position;
    }

    private void Chasing()
    {
        if (targetPlayerID == -1) return;
        GameClient.Instance.players.TryGetValue(targetPlayerID, out GameObject player);
        if (player == null) return;
        Transform targetPlayer = player.transform;
        agent.SetDestination(targetPlayer.position);
    }

    private void Attacking()
    {
        // Attacking
        Debug.Log($"Enemy: {enemyId} is attacking!");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player Entered!");
            targetPlayerID = other.GetComponent<LocalPlayer>().localID;
            Debug.Log($"PID: {targetPlayerID}");
            UpdateTargetPlayer(targetPlayerID);
        }
    }

    // private void OnTriggerStay(Collider other)
    // {
    //     if (other.CompareTag("Player") && currentState == EnemyState.WANDERING)
    //     {
    //         targetPlayerID = other.GetComponent<LocalPlayer>().localID;
    //         UpdateTargetPlayer(targetPlayerID);
    //     }
    // }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            int pID = other.GetComponent<LocalPlayer>().localID;
            if (targetPlayerID == pID)
            {
                Debug.Log("Player Exit");
                targetPlayerID = -1;
                UpdateTargetPlayer(targetPlayerID);
            }
        }
    }
}