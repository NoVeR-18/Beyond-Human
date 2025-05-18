using UnityEngine;
using UnityEngine.Tilemaps;

public class ChunkData
{
    public Vector3Int position;
    public string paletteName;
    public int tileIndexInPalette;
    public string layerName;
}
public struct TilemapEntry
{
    public Tilemap tilemap;
    public Vector3Int offset; // Смещение в клетках
}