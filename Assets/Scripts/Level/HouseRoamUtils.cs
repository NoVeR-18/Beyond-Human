using UnityEngine;
using UnityEngine.AI;

public static class HouseRoamUtils
{
    public static Vector3 GetRandomNavMeshPointInsideFloor(HouseFloor floor)
    {
        Bounds floorBounds = floor.GetBounds();
        int attempts = 10;

        for (int i = 0; i < attempts; i++)
        {
            Vector3 randomPoint = new Vector3(
                Random.Range(floorBounds.min.x + 1, floorBounds.max.x - 1),
                Random.Range(floorBounds.min.y + 1, floorBounds.max.y - 1),
                floorBounds.center.z
            );

            if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
            {
                return hit.position;
            }
        }

        return floorBounds.center;
    }
}
