using GameUtils.Utils;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class NPCSaveManager
{
    public static void SaveNPCs(List<NPCSaveData> npcData)
    {
        var container = new SaveContainer { allNpcData = npcData };
        var json = JsonUtility.ToJson(container, true);
        File.WriteAllText(SaveUtils.NPCFile, json);
    }

    public static List<NPCSaveData> LoadNPCs()
    {
        if (!File.Exists(SaveUtils.NPCFile))
            return new List<NPCSaveData>();

        var json = File.ReadAllText(SaveUtils.NPCFile);
        var container = JsonUtility.FromJson<SaveContainer>(json);
        return container.allNpcData;
    }
}
