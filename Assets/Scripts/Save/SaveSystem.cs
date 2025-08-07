using Assets.Scripts.NPC;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SaveSystem : MonoBehaviour
{
    public static SaveSystem Instance { get; private set; }

    // NPC
    private readonly List<NPCController> allNPCs = new();     // All NPC
    [SerializeField]
    private readonly List<NPCController> activeNPCs = new();  // only active NPCs
    public HashSet<string> deadNPCIDs = new();                // ID all dead NPCs

    // nav target points
    public Dictionary<string, NavTargetPoint> points = new();
    //interactables objects
    private readonly List<ISaveableInteractable> interactables = new();
    private Dictionary<string, InteractableSaveData> interactableCache = new();
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        // Импорт из BattleContext при выходе из боя
        if (!string.IsNullOrEmpty(BattleContext.Instance.returnSceneName))
        {
            foreach (var id in BattleContext.Instance.npcIDsToRemove)
                deadNPCIDs.Add(id);

            BattleContext.Instance.returnSceneName = null;
            BattleContext.Instance.npcIDsToRemove.Clear();
        }

        points.Clear();
        LoadAll();
    }
    public void RegisterInteractable(ISaveableInteractable obj)
    {
        if (!interactables.Contains(obj))
            interactables.Add(obj);
    }

    private void SaveInteractables()
    {
        foreach (var obj in interactables)
        {
            // Удаляем объект, если он уже уничтожен
            if (obj.Equals(null))
                continue;

            var id = obj.GetID();
            if (string.IsNullOrEmpty(id)) continue;

            try
            {
                var data = obj.GetSaveData();
                interactableCache[id] = data;
            }
            catch (MissingReferenceException e)
            {
                Debug.LogWarning($"Interactable {id} was destroyed but not removed from list. Skipping. Exception: {e.Message}");
            }
        }

        InteractableSaveManager.SaveInteractables(interactableCache.Values.ToList());
    }


    private void LoadInteractables()
    {
        var savedData = InteractableSaveManager.LoadInteractables();
        interactableCache.Clear();

        foreach (var data in savedData)
        {
            interactableCache[data.id] = data;

            // Проверка: был ли объект уничтожен ранее?
            if (data.isDestroyed)
            {
                // Удаляем, если объект с таким ID найден
                var obj = interactables.Find(o => o.GetID() == data.id);
                if (obj != null)
                    obj.Destroy();

                continue;
            }

            // Если объект с ID есть — загружаем данные
            var existing = interactables.Find(o => o.GetID() == data.id);
            existing?.LoadFromData(data);
        }
    }
    public void MarkAsDestroyed(InteractableObject obj)
    {
        if (obj == null) return;

        var data = obj.GetSaveData();
        data.isDestroyed = true;
        interactableCache[data.id] = data;
    }
    // Register NPC in the system
    public void RegisterNPC(NPCController npc)
    {
        if (!allNPCs.Contains(npc))
            allNPCs.Add(npc);

        if (!npc.isDead && !activeNPCs.Contains(npc))
            activeNPCs.Add(npc);
    }
    public void RegisterDeadNPC(NPCController npc)
    {
        if (!allNPCs.Contains(npc))
            allNPCs.Add(npc);

        deadNPCIDs.Add(npc.npcId);
        activeNPCs.Remove(npc);
    }


    public IReadOnlyList<NPCController> ActiveNPCs => activeNPCs;

    public NavTargetPoint GetById(string id)
    {
        points.TryGetValue(id, out var result);
        return result;
    }

    // Save all NPCs to file
    public void SaveAll()
    {
        Debug.Log("Saving NPCs...");
        List<NPCSaveData> data = new();

        foreach (var npc in allNPCs)
        {
            if (npc != null)
                data.Add(npc.GetSaveData());
        }

        NPCSaveManager.SaveNPCs(data);

        SaveInteractables();
    }

    // Load all NPCs from file and filter their states
    public void LoadAll()
    {
        var savedData = NPCSaveManager.LoadNPCs();

        foreach (var data in savedData)
        {
            var npc = allNPCs.Find(n => n.npcId == data.npcId);
            if (npc != null)
            {
                npc.LoadFromData(data);

                if (deadNPCIDs.Contains(npc.npcId) || npc.isDead)
                {
                    activeNPCs.Remove(npc);
                    npc.Destroy();
                }
                else
                {
                    if (!activeNPCs.Contains(npc))
                        activeNPCs.Add(npc);
                }
            }
        }

        // Interactables
        LoadInteractables();
    }
    private void OnApplicationQuit()
    {
        SaveAll();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F5))
        {
            SaveAll();
        }

        if (Input.GetKeyDown(KeyCode.F9))
        {
            LoadAll();
        }
    }
}
