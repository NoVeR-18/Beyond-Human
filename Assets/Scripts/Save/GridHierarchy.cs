using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridHierarchy : MonoBehaviour
{
    public Tilemap cliffs;
    public Tilemap background;
    public Tilemap ground;
    public Tilemap stairs;
    public Tilemap groundUp;
    public Tilemap backDecor;
    public Tilemap walls;
    public Tilemap colliders;
    public Tilemap decor;

    public Transform housesRoot;

    public List<HouseData> houses = new();
    private Dictionary<string, Tilemap> namedTilemaps;
    public Dictionary<string, Vector3Int> tilemapOffsets = new();
    public List<Tilemap> GetAllTilemaps()
    {
        var list = new List<Tilemap> { cliffs, background, ground, stairs, groundUp, backDecor, walls, decor };
        foreach (var house in houses)
        {
            //list.Add(house.roof);
            //list.Add(house.walls);
            //list.Add(house.floor);
            //list.Add(house.furniture);
        }
        return list;
    }


    public Dictionary<string, Tilemap> GetNamedTilemaps()
    {
        //if (namedTilemaps != null) return namedTilemaps;

        namedTilemaps = new Dictionary<string, Tilemap>()
        {
            { "cliffs", cliffs },
            { "background", background },
            { "ground", ground },
            { "stairs", stairs },
            { "groundUp", groundUp },
            { "backDecor", backDecor },
            { "walls", walls },
            { "walls", colliders },
            { "decor", decor }
        };
        // Подключаем все дома автоматически
        if (housesRoot != null)
        {
            foreach (Transform house in housesRoot)
            {
                var houseData = house.GetComponent<HouseData>();
                houseData.houseName = house.name;
                if (houseData != null)
                {
                    //AddTilemapIfExists(houseData.floor, $"House_{houseData.houseName}_Floor");
                    //AddTilemapIfExists(houseData.walls, $"House_{houseData.houseName}_Walls");
                    //AddTilemapIfExists(houseData.roof, $"House_{houseData.houseName}_Roof");
                    //AddTilemapIfExists(houseData.furniture, $"House_{houseData.houseName}_Furniture");
                }
            }
        }

        return namedTilemaps;
    }


    private void AddTilemapIfExists(Tilemap tilemap, string name)
    {
        if (tilemap != null)
        {
            namedTilemaps[name] = tilemap;
            if (tilemap.transform.parent != null && tilemap.transform.parent.parent == housesRoot)
            {
                var poss = tilemap.transform.parent.position;
                Vector3Int offset = new Vector3Int((int)poss.x, (int)poss.y, (int)poss.z);
                tilemapOffsets[name] = offset;
            }
            else
            {
                tilemapOffsets[name] = Vector3Int.zero;
            }
        }
    }
}
