using GameUtils.Utils;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class NPCSaveManager
{
    public static void SaveNPCs(List<NPCSaveData> npcData, LocationId location)
    {
        var container = new SaveContainer { allNpcData = npcData };
        var json = JsonUtility.ToJson(container, true);

        string path = Path.Combine(SaveUtils.SavePath, $"{location}_npcs.json");
        File.WriteAllText(path, json);
    }

    public static List<NPCSaveData> LoadNPCs(LocationId location)
    {
        string path = Path.Combine(SaveUtils.SavePath, $"{location}_npcs.json");

        if (!File.Exists(path))
            return new List<NPCSaveData>();

        var json = File.ReadAllText(path);
        var container = JsonUtility.FromJson<SaveContainer>(json);
        return container.allNpcData;
    }
}