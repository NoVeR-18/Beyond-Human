using System.Collections.Generic;
using UnityEngine;

public static class LadderPathfinder
{
    public static List<Vector3> FindPath(int fromFloor, int toFloor, HouseData house)
    {
        List<Vector3> path = new();

        if (house == null || house.floors == null)
            return path;

        int step = toFloor > fromFloor ? 1 : -1;

        for (int f = fromFloor; f != toFloor; f += step)
        {
            var ladder = FindLadder(house, f, step);
            if (ladder != null)
            {
                path.Add(ladder.transform.position);
            }
            else
            {
                Debug.LogWarning($"No ladder from floor {f} to {f + step} in house {house.houseName}");
                break;
            }
        }

        return path;
    }

    private static HouseLadderZone FindLadder(HouseData house, int floorIndex, int direction)
    {
        if (floorIndex < 0 || floorIndex >= house.floors.Count)
            return null;

        var floor = house.floors[floorIndex];
        if (floor.furniture == null)
            return null;

        foreach (Transform child in floor.furniture.transform)
        {
            var ladder = child.GetComponent<HouseLadderZone>();
            if (ladder != null && ladder.direction == direction && ladder.floorManager.house == house)
            {
                return ladder;
            }
        }

        return null;
    }
}
