using UnityEngine;

public class FloorManager : MonoBehaviour
{
    public HouseData house;
    private int currentFloor = 0;

    public void ChangeFloor(int delta)
    {
        int newFloor = currentFloor + delta;

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
}
