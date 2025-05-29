using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum HousePrivacyType
{
    Public,
    PrivateAlways,
    PrivateScheduled
}


[System.Serializable]
public class HouseFloor
{
    public Tilemap walls;
    public Tilemap floor;
    public Tilemap furniture;

    public Bounds GetBounds()
    {
        if (walls == null)
        {
            Debug.LogWarning("Tilemap walls is null!");
            return new Bounds(Vector3.zero, Vector3.zero);
        }

        // cellBounds — в координатах клеток, поэтому переводим в мир
        var bounds = walls.cellBounds;
        var min = walls.CellToWorld(bounds.min);
        var max = walls.CellToWorld(bounds.max);

        Vector3 center = (min + max) * 0.5f;
        Vector3 size = max - min;

        // Расширим немного по высоте, если нужно
        size.y = 3f; // произвольно

        return new Bounds(center, size);
    }
}

[System.Serializable]
public class HouseData : MonoBehaviour
{
    public string houseName;
    public Tilemap roof;
    public int entranceFloor = 0;

    public List<HouseFloor> floors = new();
    public List<Transform> entrances = new();
    [Header("Privacy")]
    public HousePrivacyType privacyType;

    public Vector2Int privateHours = new Vector2Int(20, 6);

    public bool IsPrivateNow()
    {
        int hour = TimeManager.Instance.CurrentTime.Hour; // предполагается глобальное время
        if (privacyType == HousePrivacyType.PrivateAlways) return true;
        if (privacyType == HousePrivacyType.PrivateScheduled)
            return hour >= privateHours.x || hour < privateHours.y;
        return false;
    }

    public Transform GetClosestEntrance(Vector3 worldPos)
    {
        return entrances.OrderBy(e => Vector3.Distance(e.position, worldPos)).FirstOrDefault();
    }

    public Vector3 GetEntrancePosition()
    {
        return entrances[0].position;
    }
}