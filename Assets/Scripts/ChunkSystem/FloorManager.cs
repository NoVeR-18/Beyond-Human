using Assets.Scripts.NPC;
using UnityEngine;

public class FloorManager : MonoBehaviour
{
    public HouseData house;
    private int currentFloor = 0;

    private void Start()
    {
        if (house == null)

            house = GetComponent<HouseData>();
        if (house.floors == null || house.floors.Count > 1)
        {
            for (int i = 1; i < house.floors.Count; i++)
            {
                SetFloorVisibility(house.floors[i], false);
            }
        }
        SetFloorVisibility(house.floors[house.entranceFloor], true);
    }


    public void ChangeFloor(int delta)
    {
        int newFloor = delta; currentFloor = delta;

        if (newFloor < 0 || newFloor >= house.floors.Count) return;

        for (int i = 0; i < house.floors.Count; i++)
        {
            bool isCurrent = i == newFloor;
            SetFloorVisibility(house.floors[i], isCurrent);
        }

        currentFloor = newFloor;
    }

    void SetFloorVisibility(HouseFloor floor, bool visible)
    {
        if (floor.walls != null) floor.walls.gameObject.SetActive(visible);
        if (floor.floor != null) floor.floor.gameObject.SetActive(visible);
        if (floor.furniture != null)
        {
            floor.furniture.gameObject.SetActive(visible);
            foreach (Transform child in floor.furniture.transform)
                child.gameObject.SetActive(visible);
        }
    }
    public void UpdateNPCVisibility(NPCController npc)
    {
        bool isOnCurrentFloor = npc.CurrentFloor == currentFloor;
        npc.SetVisible(isOnCurrentFloor);
    }
}
