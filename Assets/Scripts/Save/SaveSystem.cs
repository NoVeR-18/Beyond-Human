using Assets.Scripts.NPC;
using GameWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class SaveSystem : MonoBehaviour
{
    public static SaveSystem Instance { get; private set; }

    [SerializeField] private PlayerController player;
    private PlayerSaveData playerCache;
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

    private Dictionary<InteractableObject, string> prefabToId = new();
    private Dictionary<string, InteractableObject> idToPrefab = new();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        prefabToId.Clear();
        idToPrefab.Clear();
    }

    public string GetPrefabIdForObject(InteractableObject obj)
    {
        // Сравниваем с оригинальными префабами по типу
        foreach (var kvp in prefabToId)
        {
            if (obj.name.StartsWith(kvp.Key.name)) // Проверка по имени (без "(Clone)")
                return kvp.Value;
        }
        return null;
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
            string id = obj?.GetID();

            // Если id нет — пропускаем
            if (string.IsNullOrEmpty(id)) continue;

            // Если объект уничтожен в сцене — ставим флаг isDestroyed (создадим запись, если её нет)
            if (obj == null || obj.Equals(null))
            {
                if (interactableCache.TryGetValue(id, out var existing))
                {
                    existing.isDestroyed = true;
                    interactableCache[id] = existing;
                }
                else
                {
                    interactableCache[id] = new InteractableSaveData
                    {
                        id = id,
                        isDestroyed = true
                    };
                }
                continue;
            }

            try
            {
                var data = obj.GetSaveData();
                if (string.IsNullOrEmpty(data.id))
                    data.id = id;

                // просто добавляем/заменяем запись в кеше
                interactableCache[id] = data;
            }
            catch (MissingReferenceException e)
            {
                Debug.LogWarning($"Interactable {id} was destroyed but not removed from list. Skipping. Exception: {e.Message}");
            }
        }

        // если кеш пуст — можно не вызывать сохранение, но это опционально
        InteractableSaveManager.SaveInteractables(interactableCache.Values.ToList());
    }



    private void LoadInteractables()
    {
        var savedData = InteractableSaveManager.LoadInteractables();
        interactableCache.Clear();

        foreach (var data in savedData)
        {
            interactableCache[data.id] = data;

            if (data.isDestroyed)
                continue;

            var existing = interactables.Find(o => o != null && o.GetID() == data.id);

            if (existing == null)
            {
                // грузим по ключу из Addressables
                var handle = Addressables.LoadAssetAsync<GameObject>(data.prefabId);
                handle.Completed += op =>
                {
                    if (op.Status == AsyncOperationStatus.Succeeded)
                    {
                        var prefab = op.Result;
                        var spawned = Instantiate(prefab, data.position, data.rotation)
                                        .GetComponent<InteractableObject>();
                        if (spawned != null)
                        {
                            spawned.LoadFromData(data);
                            RegisterInteractable(spawned);
                        }
                        else
                        {
                            Debug.LogError($"Префаб {data.prefabId} не содержит InteractableObject");
                        }
                    }
                    else
                    {
                        Debug.LogError($"Не удалось загрузить префаб с ключом {data.prefabId}");
                    }
                };
            }
            else
            {
                existing.LoadFromData(data);
            }
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
            {
                var npcData = npc.GetSaveData();
                npcData.locationId = LocationManager.Instance.GetCurrentLocation();
                data.Add(npcData);
            }
        }

        NPCSaveManager.SaveNPCs(data, LocationManager.Instance.GetCurrentLocation());

        SavePlayer();
        SaveInteractables();
    }
    private void SavePlayer()
    {
        if (player != null)
        {
            playerCache = player.GetSaveData();
            PlayerSaveManager.SavePlayer(playerCache);
        }
    }

    private void LoadPlayer()
    {
        var data = PlayerSaveManager.LoadPlayer();
        if (data != null && player != null)
        {
            player.LoadFromData(data);
        }
    }
    public void PlayerEntrance(Transform transform)
    {
        player.transform.position = transform.position;
    }

    // Load all NPCs from file and filter their states
    public void LoadAll()
    {
        var currentLoc = LocationManager.Instance.GetCurrentLocation();
        var savedData = NPCSaveManager.LoadNPCs(currentLoc);

        foreach (var data in savedData.Where(d => d.locationId == currentLoc))
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

        LoadPlayer();
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
