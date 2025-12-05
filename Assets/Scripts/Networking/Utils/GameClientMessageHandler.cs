using System;
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
            case "UPD_POS":
                HandleUpdatePosition(parts);
                break;
            case "UPD_ANIM":
                HandleUpdateAnimation(parts);
                break;
            case "UPD_SHOOT":
                HandleShoot(parts);
                break;
            case "SPAWN_ENEMY":
                HandleEnemySpawn(parts);
                break;
            case "UPD_ENEMY":
                HandleEnemyUpdate(parts);
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
        if (parts[0] != "UPD_POS") return;
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

    private static void HandleUpdateAnimation(string[] parts)
    {
        if (parts[0] != "UPD_ANIM") return;
        int id = int.Parse(parts[1]);
        string paramName = parts[2];
        float val = float.Parse(parts[3]);

        GameClient.Instance.OnPlayerAnimate(id, paramName, val);
    }

    private static void HandleShoot(string[] parts)
    {
        if (parts[0] != "UPD_SHOOT") return;
        int id = int.Parse(parts[1]);

        // Example: SHOOT:ID:X,Y,Z:X,Y,Z,W:X,Y,Z:-1

        string[] shootPtParts = parts[2].Split(',');
        string[] shootPtRotParts = parts[3].Split(',');
        string[] hitPtParts = parts[4].Split(',');
        int targetId = int.Parse(parts[5]);

        Vector3 shootPoint = new Vector3(float.Parse(shootPtParts[0]), float.Parse(shootPtParts[1]), float.Parse(shootPtParts[2]));
        Quaternion shootPtRot = new Quaternion(float.Parse(shootPtRotParts[0]), float.Parse(shootPtRotParts[1]), float.Parse(shootPtRotParts[2]), float.Parse(shootPtRotParts[3]));
        Vector3 hitPoint = new Vector3(float.Parse(hitPtParts[0]), float.Parse(hitPtParts[1]), float.Parse(hitPtParts[2]));

        GameClient.Instance.OnPlayerShoot(id, shootPoint, shootPtRot, hitPoint, targetId);
    }

    private static void HandleEnemySpawn(string[] parts)
    {
        if (parts[0] != "SPAWN_ENEMY") return;
        int id = int.Parse(parts[1]);
        string[] posParts = parts[2].Split(',');

        // Example: SPAWN_ENEMY:1:x,y,z:2:3:CHASING
        Vector3 pos = new Vector3(float.Parse(posParts[0]), float.Parse(posParts[1]), float.Parse(posParts[2]));
        float rotY = float.Parse(parts[3]);
        int targetPID = int.Parse(parts[4]);
        EnemyState state = (EnemyState)Enum.Parse(typeof(EnemyState), parts[5]);

        // Update enemy here
        GameClient.Instance.OnEnemySpawn(id, pos, rotY, targetPID, state);
    }

    private static void HandleEnemyUpdate(string[] parts)
    {
        if (parts[0] != "UPD_ENEMY") return;
        int id = int.Parse(parts[1]);
        int targetPID = int.Parse(parts[2]);
        EnemyState state = (EnemyState)Enum.Parse(typeof(EnemyState), parts[3]);

        // Update enemy here
        GameClient.Instance.OnEnemyUpdate(id, targetPID, state);
    }

    #endregion
}