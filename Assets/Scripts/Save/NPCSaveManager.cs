using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class NPCSaveManager
{
    private static string SavePath => Path.Combine(Application.persistentDataPath, "npc_save.json");

    public static void SaveNPCs(List<NPCSaveData> npcData)
    {
        var container = new NPCSaveContainer { allNpcData = npcData };
        var json = JsonUtility.ToJson(container, true);
        File.WriteAllText(SavePath, json);
    }

    public static List<NPCSaveData> LoadNPCs()
    {
        if (!File.Exists(SavePath))
            return new List<NPCSaveData>();

        var json = File.ReadAllText(SavePath);
        var container = JsonUtility.FromJson<NPCSaveContainer>(json);
        return container.allNpcData;
    }
}
