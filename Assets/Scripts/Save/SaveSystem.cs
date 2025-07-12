using Assets.Scripts.NPC;
using System.Collections.Generic;
using UnityEngine;

public class SaveSystem : MonoBehaviour
{
    public static SaveSystem Instance { get; private set; }
    //NPS
    private readonly List<NPCController> activeNPCs = new();
    public HashSet<string> deadNPCIDs = new();

    //Points
    public Dictionary<string, NavTargetPoint> points = new();
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

    }
    private void Start()
    {
        if (!string.IsNullOrEmpty(BattleContext.Instance.returnSceneName))
        {
            foreach (var ids in BattleContext.Instance.npcIDsToRemove)
            {
                deadNPCIDs.Add(ids);
            }

            BattleContext.Instance.returnSceneName = null;
            BattleContext.Instance.npcIDsToRemove.Clear();

        }
        points.Clear();

        LoadAll();
    }
    public void RegisterNPC(NPCController npc)
    {
        if (!activeNPCs.Contains(npc))
            activeNPCs.Add(npc);
    }

    public NavTargetPoint GetById(string id)
    {
        points.TryGetValue(id, out var result);
        return result;
    }

    public void SaveAll()
    {
        Debug.Log("Saving NPCs...");
        List<NPCSaveData> data = new();

        foreach (var npc in activeNPCs)
        {
            data.Add(npc.GetSaveData());
        }

        NPCSaveManager.SaveNPCs(data);
    }

    public void LoadAll()
    {
        var savedData = NPCSaveManager.LoadNPCs();
        foreach (var data in savedData)
        {
            var npc = activeNPCs.Find(n => n.npcId == data.npcId);
            if (npc != null)
            {
                if (deadNPCIDs.Contains(npc.npcId))
                {
                    npc.isDead = data.isDead;
                    activeNPCs.Remove(npc);
                    npc.Destroy();
                }
                else
                    npc.LoadFromData(data);
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
