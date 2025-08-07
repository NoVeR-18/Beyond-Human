using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class InteractableSaveManager
{
    private static string SavePath => Path.Combine(Application.persistentDataPath, "interactables_save.json");

    public static void SaveInteractables(List<InteractableSaveData> data)
    {
        var container = new InteractablesSaveContainer { allInteractables = data };
        var json = JsonUtility.ToJson(container, true);
        File.WriteAllText(SavePath, json);
    }

    public static List<InteractableSaveData> LoadInteractables()
    {
        if (!File.Exists(SavePath))
            return new List<InteractableSaveData>();

        var json = File.ReadAllText(SavePath);
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
    public bool isOpened;
    public bool isDestroyed = false;
    public List<InventoryItem> items = new(); // содержимое (если это сундук)



    public Vector3 position;
    public Quaternion rotation;

}
public interface ISaveableInteractable
{
    string GetID();
    InteractableSaveData GetSaveData();
    void LoadFromData(InteractableSaveData data);
    void Destroy();
}
