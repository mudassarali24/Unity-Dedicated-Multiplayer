using System.Collections.Generic;
using UnityEngine;

public class GameClient : MonoBehaviour
{
    [Header("References")]
    public GameObject localPlayerObj;
    public GameObject remotePlayerObj;

    public int playerID { get; private set; }
    public Dictionary<int, GameObject> players = new Dictionary<int, GameObject>();
    public static GameClient Instance;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        TcpClientManager.Instance.Connect("127.0.0.1", 7777);
    }

    public void AssignID(int pID)
    {
        playerID = pID;
    }

    public void OnPlayerSpawn(int id, Vector3 pos)
    {
        if (players.ContainsKey(id)) return;
        GameObject objectToSpawn = (id == playerID) ? localPlayerObj : remotePlayerObj;
        GameObject obj = Instantiate(objectToSpawn, pos, Quaternion.identity);
        players.Add(id, obj);
        obj.GetComponent<LocalPlayer>().isLocalPlayer = (id == playerID);

        if (id == playerID)
            Debug.Log("Spawned local player!");
        else
            Debug.Log("Spawned remote player " + id);
    }

    public void OnPlayerMove(int id, Vector3 pos)
    {
        if (!players.ContainsKey(id)) return;
        players[id].transform.position = pos;
    }
}