using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilePaletteCache
{
    private static Dictionary<string, TileBase> tileDict;

    public static void LoadPalette()
    {
        var palette = Resources.LoadAll<TilePaletteData>("");
        if (palette == null)
        {
            Debug.LogError($"�� ������� ��������� ������� �� ����: ");
            return;
        }
        foreach (var tile in palette)
        {
            if (tile == null)
            {
                Debug.LogError($"������� �������� ������ �������.");
                return;
            }
            tileDict = tile.tiles.ToDictionary(tile => tile.name, tile => tile);
        }

    }

    public static TileBase GetTileByName(string name)
    {
        if (tileDict == null)
        {
            Debug.LogError("TilePalette �� ��������!");
            return null;
        }

        tileDict.TryGetValue(name, out var tile);
        return tile;
    }
}
