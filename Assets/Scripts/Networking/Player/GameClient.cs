using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class GameClient : MonoBehaviour
{
    [Header("References")]
    public GameObject localPlayerObj;
    public GameObject remotePlayerObj;

    public int playerID { get; private set; }
    public Dictionary<int, GameObject> players = new Dictionary<int, GameObject>();
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
}