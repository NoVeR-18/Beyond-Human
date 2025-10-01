using GameUtils.Utils;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class InteractableSaveManager
{
    private static string SavePath => Path.Combine(Application.persistentDataPath, "interactables_save.json");

    public static void SaveInteractables(List<InteractableSaveData> data, LocationId location)
    {
        var container = new InteractablesSaveContainer { allInteractables = data };
        var json = JsonUtility.ToJson(container, true);

        string path = Path.Combine(SaveUtils.SavePath, $"{location}_interactables.json");
        File.WriteAllText(path, json);
    }

    public static List<InteractableSaveData> LoadInteractables(LocationId location)
    {
        string path = Path.Combine(SaveUtils.SavePath, $"{location}_interactables.json");
        if (!File.Exists(path))
            return new List<InteractableSaveData>();

        var json = File.ReadAllText(path);
        var container = JsonUtility.FromJson<InteractablesSaveContainer>(json);
        return container.allInteractables;
    }
}
[System.Serializable]
public class InteractablesSaveContainer
{
    public List<InteractableSaveData> allInteractables = new();
}
[System.Serializable]
public class InteractableSaveData
{
    public string id; // уникальный ID объекта
    public string prefabId;
    public bool isOpened;
    public bool isDestroyed = false;
    public List<InventoryItemSaveData> items = new(); // содержимое (если это сундук)


    public Vector3 position;
    public Quaternion rotation;
    public LocationId locationId;

}
public interface ISaveableInteractable
{
    string GetID();
    InteractableSaveData GetSaveData();
    void LoadFromData(InteractableSaveData data);
    void Destroy();
}
[System.Serializable]
public class InteractablePrefabEntry
{
    public string prefabId;
    public InteractableObject prefab;
}
[System.Serializable]
public class InventoryItemSaveData
{
    public string itemKey; // itemName из Item
    public int quantity;
}
