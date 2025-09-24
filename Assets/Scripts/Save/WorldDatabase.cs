using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WorldDatabase", menuName = "Game/World Database")]
public class WorldDatabase : ScriptableObject
{
    [SerializeField]
    private Sprite worldMap;
    [SerializeField]
    private List<LocationData> locations = new();

    [SerializeField]
    private List<LocationsBackGrounds> locationsBackGrounds = new();

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

    public Sprite GetRandomBackGround(LocationId locationId)
    {
        var backgrounds = locationsBackGrounds.FindAll(bg => bg.locationId == locationId);
        return backgrounds.Count > 0 ? backgrounds[UnityEngine.Random.Range(0, backgrounds.Count)].background : null;
    }

    public Sprite GetWorldMap()
    {
        return worldMap;
    }

    public IReadOnlyList<LocationData> Locations => locations;
}

[Serializable]
public class LocationsBackGrounds
{
    public LocationId locationId;
    public Sprite background;
}
