using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridHierarchy : MonoBehaviour
{
    public Tilemap cliffs;
    public Tilemap background;
    public Tilemap ground;
    public Tilemap backDecor;
    public Tilemap walls;
    public Tilemap decor;


    public List<HouseData> houses = new();

    public List<Tilemap> GetAllTilemaps()
    {
        var list = new List<Tilemap> { cliffs, background, ground, backDecor, walls, decor };
        foreach (var house in houses)
        {
            list.Add(house.roof);
            list.Add(house.walls);
            list.Add(house.floor);
            list.Add(house.furniture);
        }
        return list;
    }
    public Dictionary<string, Tilemap> GetNamedTilemaps()
    {
        var dict = new Dictionary<string, Tilemap>
        {
            { "cliffs", cliffs },
            { "background", background },
            { "ground", ground },
            { "backDecor", backDecor },
            { "walls", walls },
            { "decor", decor }
        };

        foreach (var house in houses)
        {
            dict[$"{house.houseName}_roof"] = house.roof;
            dict[$"{house.houseName}_walls"] = house.walls;
            dict[$"{house.houseName}_floor"] = house.floor;
            dict[$"{house.houseName}_furniture"] = house.furniture;
        }

        return dict;
    }
}
[System.Serializable]
public class HouseData : MonoBehaviour
{
    public string houseName;
    public GameObject houseObject;
    public Tilemap roof;
    public Tilemap walls;
    public Tilemap floor;
    public Tilemap furniture;
}
