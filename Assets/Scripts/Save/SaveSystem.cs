using Assets.Scripts.NPC;
using System.Collections.Generic;
using UnityEngine;

public class SaveSystem : MonoBehaviour
{
    public static SaveSystem Instance { get; private set; }

    private readonly List<NPCController> activeNPCs = new();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    private void Start()
    {
        LoadAll();
    }
    public void RegisterNPC(NPCController npc)
    {
        if (!activeNPCs.Contains(npc))
            activeNPCs.Add(npc);
    }

    public void SaveAll()
    {
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
