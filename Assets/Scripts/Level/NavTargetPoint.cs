using UnityEngine;

public class NavTargetPoint : MonoBehaviour
{
    public HouseData house;      // null если это улица
    public int floorIndex = 0;   // игнорируется, если house == null

    public bool IsInsideHouse => house != null;
}