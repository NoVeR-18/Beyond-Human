
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Tile Palette Data")]
public class TilePaletteData : ScriptableObject
{
    public string paletteName;
    public List<TileBase> tiles;

    public int GetTileIndex(TileBase tile) => tiles.IndexOf(tile);
    public TileBase GetTileByIndex(int index) => (index >= 0 && index < tiles.Count) ? tiles[index] : null;

    public TileBase GetTileByName(string name)
    {
        return tiles.Find(t => t != null && t.name == name);
    }
}