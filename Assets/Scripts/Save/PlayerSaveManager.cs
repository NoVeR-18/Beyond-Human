using System.IO;
using UnityEngine;

public static class PlayerSaveManager
{
    private static string SavePath => Path.Combine(Application.persistentDataPath, "player.json");

    public static void SavePlayer(PlayerSaveData data)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(SavePath, json);
    }

    public static PlayerSaveData LoadPlayer()
    {
        if (!File.Exists(SavePath)) return null;
        string json = File.ReadAllText(SavePath);
        return JsonUtility.FromJson<PlayerSaveData>(json);
    }
}
