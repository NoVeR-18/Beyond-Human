using NPCEnums;
using System.Collections.Generic;
using UnityEngine;

public class FactionManager : MonoBehaviour
{
    public static FactionManager Instance { get; private set; }

    public List<FactionData> factions;
    private Dictionary<FactionType, FactionData> factionDict;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        //DontDestroyOnLoad(gameObject);

        factionDict = new();
        foreach (var faction in factions)
        {
            if (!factionDict.ContainsKey(faction.factionType))
                factionDict.Add(faction.factionType, faction);
            else
                Debug.LogWarning($"Duplicate factionType {faction.factionType} in FactionManager");
        }
    }

    public FactionData GetFaction(FactionType type)
    {
        factionDict.TryGetValue(type, out var faction);
        return faction;
    }

    // 🔄Change reputation between factions
    public void ModifyReputation(FactionType source, FactionType target, int delta)
    {
        var sourceFaction = GetFaction(source);
        var targetFaction = GetFaction(target);
        if (sourceFaction == null || targetFaction == null)
        {
            Debug.LogWarning($"Faction not found for modifying reputation: {source} -> {target}");
            return;
        }

        sourceFaction.ModifyAttitudeTowards(targetFaction, delta);
        targetFaction.ModifyAttitudeTowards(sourceFaction, delta); // reciprocal change
    }

    // similar to ModifyReputation, but specifically for Player faction
    public void ModifyReputationWithPlayer(FactionType source, int delta)
    {
        ModifyReputation(source, FactionType.Player, delta);
    }
}
