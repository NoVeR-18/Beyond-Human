using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public class HouseData : MonoBehaviour
{
    public string houseName;
    public Tilemap roof;
    public Tilemap walls;
    public Tilemap floor;
    public Tilemap furniture;
}