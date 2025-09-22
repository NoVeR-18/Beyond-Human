using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WorldDatabase", menuName = "Game/World Database")]
public class WorldDatabase : ScriptableObject
{
    [SerializeField]
    private List<LocationData> locations = new();

    private Dictionary<LocationId, LocationData> lookup;

    public LocationData GetLocation(LocationId id)
    {
        if (lookup == null) BuildLookup();
        lookup.TryGetValue(id, out var data);
        return data;
    }

    private void BuildLookup()
    {
        lookup = new Dictionary<LocationId, LocationData>();
        foreach (var loc in locations)
        {
            if (!lookup.ContainsKey(loc.id))
                lookup[loc.id] = loc;
            else
                Debug.LogWarning($"Duplicate location id: {loc.id}");
        }
    }

    public string GetScene(LocationId saved)
    {

        if (lookup == null) BuildLookup();
        lookup.TryGetValue(saved, out var data);
        return data.SceneName;
    }

    public IReadOnlyList<LocationData> Locations => locations;
}
