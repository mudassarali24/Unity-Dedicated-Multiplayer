using System.Collections.Generic;
using UnityEngine;

public static class GameClientMessageHandler
{
    public static Dictionary<int, Transform> players = new Dictionary<int, Transform>();

    public static void Handle(string msg)
    {
        if (msg.StartsWith("UPD:"))
        {
            string[] parts = msg.Split(':');
            int id = int.Parse(parts[1]);
            float x = float.Parse(parts[2]);
            float y = float.Parse(parts[3]);
            float z = float.Parse(parts[4]);

            if (!players.ContainsKey(id))
            {
                var obj = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                obj.name = $"Player_{id}";
                players[id] = obj.transform;
            }

            players[id].transform.position = new Vector3(x, y, z);
        }
    }
}