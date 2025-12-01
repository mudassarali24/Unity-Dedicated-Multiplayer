using System.Collections.Generic;
using UnityEngine;

public static class GameClientMessageHandler
{
    public static Dictionary<int, Transform> players = new Dictionary<int, Transform>();

    public static void Handle(string msg)
    {
        string[] parts = msg.Split(':');

        switch (parts[0])
        {
            case "ASSIGN_ID":
                HandleAssignedID(parts);
                break;
            case "SPAWN":
                HandleSpawn(parts);
                break;
            case "UPD":
                HandleUpdatePosition(parts);
                break;
        }
    }


    #region MESSAGE_HANDLERS

    private static void HandleAssignedID(string[] parts)
    {
        if (parts[0] != "ASSIGN_ID") return;
        int id = int.Parse(parts[1]);
        GameClient.Instance.AssignID(id);
    }

    private static void HandleSpawn(string[] parts)
    {
        if (parts[0] != "SPAWN") return;
        int id = int.Parse(parts[1]);
        Vector3 pos = new Vector3();
        Vector3 rot = new Vector3();
        //position
        pos.x = float.Parse(parts[2]);
        pos.y = float.Parse(parts[3]);
        pos.z = float.Parse(parts[4]);
        //rotation
        rot.x = float.Parse(parts[5]);
        rot.y = float.Parse(parts[6]);
        rot.z = float.Parse(parts[7]);

        GameClient.Instance.OnPlayerSpawn(id, pos, rot);
    }

    private static void HandleUpdatePosition(string[] parts)
    {
        if (parts[0] != "UPD") return;
        int id = int.Parse(parts[1]);
        Vector3 pos = new Vector3();
        Vector3 rot = new Vector3();
        //position
        pos.x = float.Parse(parts[2]);
        pos.y = float.Parse(parts[3]);
        pos.z = float.Parse(parts[4]);
        //rotation
        rot.x = float.Parse(parts[5]);
        rot.y = float.Parse(parts[6]);
        rot.z = float.Parse(parts[7]);

        GameClient.Instance.OnPlayerMove(id, pos, rot);
    }

    #endregion
}