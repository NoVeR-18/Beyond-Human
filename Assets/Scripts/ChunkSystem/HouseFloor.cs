using UnityEngine;
using UnityEngine.Tilemaps;

public class HouseFloor : MonoBehaviour
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
