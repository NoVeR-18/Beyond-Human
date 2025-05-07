using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
[System.Serializable]
public class HouseFloor
{
    public Tilemap walls;
    public Tilemap floor;
    public Tilemap furniture;
}

[System.Serializable]
public class HouseData : MonoBehaviour
{
    public string houseName;
    public Tilemap roof;
    public List<HouseFloor> floors = new(); // По индексу = номер этажа
}