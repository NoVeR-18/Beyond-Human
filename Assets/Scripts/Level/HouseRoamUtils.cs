using UnityEngine;
using UnityEngine.AI;

public static class HouseRoamUtils
{
    public static Vector3 GetRandomNavMeshPointInsideFloor(HouseFloor floor, int maxAttempts = 10)
    {
        if (floor.floor == null)
        {
            Debug.LogWarning("Floor tilemap missing!");
            return Vector3.zero;
        }

        var bounds = floor.floor.cellBounds;
        var tilemap = floor.floor;

        for (int i = 0; i < maxAttempts; i++)
        {
            int x = Random.Range(bounds.xMin, bounds.xMax);
            int y = Random.Range(bounds.yMin, bounds.yMax);
            Vector3 worldPoint = tilemap.GetCellCenterWorld(new Vector3Int(x, y, 0));

            if (NavMesh.SamplePosition(worldPoint, out NavMeshHit hit, 0.5f, NavMesh.AllAreas))
            {
                return hit.position;
            }
        }

        // fallback: возвращаем текущую позицию NPC
        Debug.LogWarning("Couldn't find valid NavMesh position inside floor.");
        return Vector3.zero;
    }
}
