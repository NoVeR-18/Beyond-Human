using Assets.Scripts.NPC;
using System.Collections.Generic;
using UnityEngine;

public class SaveSystem : MonoBehaviour
{
    public static SaveSystem Instance { get; private set; }

    // NPC
    private readonly List<NPCController> allNPCs = new();     // All NPC
    [SerializeField]
    private readonly List<NPCController> activeNPCs = new();  // only active NPCs
    public HashSet<string> deadNPCIDs = new();                // ID all dead NPCs

    // Навигационные точки
    public Dictionary<string, NavTargetPoint> points = new();

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
