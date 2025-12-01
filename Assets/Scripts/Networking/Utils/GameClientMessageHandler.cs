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
        float x = float.Parse(parts[2]);
        float y = float.Parse(parts[3]);
        float z = float.Parse(parts[4]);

        GameClient.Instance.OnPlayerSpawn(id, new Vector3(x, y, z));
    }

    private static void HandleUpdatePosition(string[] parts)
    {
        if (parts[0] != "UPD") return;
        int id = int.Parse(parts[1]);
        float x = float.Parse(parts[2]);
        float y = float.Parse(parts[3]);
        float z = float.Parse(parts[4]);

        GameClient.Instance.OnPlayerMove(id, new Vector3(x, y, z));
    }

    #endregion
}