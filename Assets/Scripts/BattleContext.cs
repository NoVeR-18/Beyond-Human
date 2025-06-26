using BattleSystem;
using System.Collections.Generic;
using UnityEngine;

public class BattleContext : MonoBehaviour
{
    public List<BattleParticipantData> Participants = new();
    public string returnSceneName;
    public string returnPointID; // куда вернуться
    public List<string> npcIDsToRemove; // NPC которых нужно удалить по завершении

    public static BattleContext Instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject); // не даём пересоздать
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}

[System.Serializable]
public class BattleParticipantData
{
    public GameObject prefab;
    public BattleTeam team;
    public CharacterStats stats;
    public List<AbilityData> abilities = new();
}
