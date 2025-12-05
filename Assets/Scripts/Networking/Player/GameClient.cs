using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class GameClient : MonoBehaviour
{
    [Header("References")]
    public GameObject localPlayerObj;
    public GameObject remotePlayerObj;
    public GameObject enemyObj;

    public int playerID { get; private set; }
    public Dictionary<int, GameObject> players = new Dictionary<int, GameObject>();
    public Dictionary<int, GameObject> enemies = new Dictionary<int, GameObject>();
    public Dictionary<int, LocalPlayer> localPlayers = new Dictionary<int, LocalPlayer>();
    public static GameClient Instance;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        TcpClientManager.Instance.Connect("192.168.100.186", 7777);
    }

    public void AssignID(int pID)
    {
        playerID = pID;
    }
    public void OnPlayerSpawn(int id, Vector3 pos, Vector3 rot)
    {
        if (players.ContainsKey(id)) return;
        GameObject objectToSpawn = (id == playerID) ? localPlayerObj : remotePlayerObj;
        GameObject obj = Instantiate(objectToSpawn, pos, Quaternion.Euler(rot));
        LocalPlayer localPlayer = obj.GetComponent<LocalPlayer>();
        players.Add(id, obj);
        localPlayers.Add(id, localPlayer);
        localPlayer.isLocalPlayer = (id == playerID);
        localPlayer.localID = id;

        if (id == playerID)
            Debug.Log("Spawned local player!");
        else
            Debug.Log("Spawned remote player " + id);
    }

    public void OnPlayerMove(int id, Vector3 pos, Vector3 rot)
    {
        if (!players.ContainsKey(id)) return;
        // Ignore local movement
        if (GameManager.Instance.localPlayer == null) return;
        if (GameManager.Instance.localPlayer != null &&
            GameManager.Instance.localPlayer.localID == id) return;
        NetworkTransform nt = players[id].GetComponent<NetworkTransform>();
        nt.SetTarget(pos, Quaternion.Euler(rot));
    }

    public void OnPlayerAnimate(int id, string paramName, float val)
    {
        if (!players.ContainsKey(id)) return;
        // Ignore local movement
        if (GameManager.Instance.localPlayer == null) return;
        if (GameManager.Instance.localPlayer != null &&
            GameManager.Instance.localPlayer.localID == id) return;
        localPlayers[id].characterController.UpdateAnimators(paramName, val);
    }

    public void OnPlayerShoot(int id, Vector3 shootPt, Quaternion shootPtRot, Vector3 hitPt, int targetId)
    {
        if (!players.ContainsKey(id)) return;
        // Ignore local movement
        if (GameManager.Instance.localPlayer == null) return;
        if (GameManager.Instance.localPlayer != null &&
            GameManager.Instance.localPlayer.localID == id) return;

        // Show effects at correct positions
        EffectsManager.Instance.SpawnMuzzleFlash(shootPt, shootPtRot);
        EffectsManager.Instance.SpawnHitImpact(hitPt);

        if (Vector3.Distance(GameManager.Instance.localPlayer.transform.position, hitPt) <= 2.5f)
        {
            Vector3 pos = GameManager.Instance.localPlayer.transform.position;
            pos.y = -0.7f;
            EffectsManager.Instance.SpawnPlayerHitEffect(pos);
        }
    }

    public void OnEnemySpawn(int id, Vector3 pos, float rotY, int targetPlayerID, EnemyState currState)
    {
        if (enemies.ContainsKey(id)) return;
        GameObject objectToSpawn = enemyObj;
        GameObject obj = Instantiate(objectToSpawn, pos, Quaternion.Euler(0f, rotY, 0f));
        NetworkEnemy enemy = obj.GetComponent<NetworkEnemy>();
        enemy.enemyId = id;
        enemy.targetPlayerID = targetPlayerID;
        enemy.currentState = currState;
        enemies.Add(id, obj);

        Debug.Log($"Spawned Enemy: {id}");
    }

    public void OnEnemyUpdate(int id, int targetPlayerID, EnemyState currState)
    {
        if (!enemies.ContainsKey(id)) return;
        GameObject obj = enemies[id];
        NetworkEnemy enemy = obj.GetComponent<NetworkEnemy>();
        enemy.targetPlayerID = targetPlayerID;
        enemy.currentState = currState;
    }
}