using System.Collections.Generic;
using UnityEngine;

public static class LadderPathfinder
{
    public static List<Vector3> FindPath(int fromFloor, int toFloor, HouseData house)
    {
        List<Vector3> path = new();
        int step = toFloor > fromFloor ? 1 : -1;

        for (int f = fromFloor; f != toFloor; f += step)
        {
            int nextFloor = f + step;
            HouseLadderZone ladder = FindLadder(house, f, nextFloor);

            if (ladder != null)
            {
                path.Add(ladder.transform.position);
            }
            else
            {
                Debug.LogError($"[LadderPathfinder] Не найдена лестница из {f} на {nextFloor} этаж в доме {house.name}");
                break; // чтобы не продолжать построение некорректного пути
            }
        }

        return path;
    }


    public static HouseLadderZone FindLadder(HouseData house, int fromFloor, int toFloor)
    {
        HouseLadderZone[] allLadders = house.GetComponentsInChildren<HouseLadderZone>(true); // ← ВАЖНО: true включает неактивные

        foreach (var ladder in allLadders)
        {
            if (ladder.floorManager == null) continue;
            if (ladder.floorManager.house != house) continue;

            if (ladder.IndextFloor == fromFloor && ladder.TargetFloor == toFloor)
            {
                return ladder;
            }
        }

        Debug.LogWarning($"Не найдена лестница из {fromFloor} на {toFloor} этаж в доме {house.name}");
        return null;
    }

}
