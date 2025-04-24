using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Chunk Data")]
[System.Serializable]
public class ChunkData
{
    public Vector2Int chunkPosition;
    public List<ChunkTile> tiles = new();
}
[System.Serializable]
public class LayerData
{
    public string layerName;
    public List<TileData> tiles = new();
}
[System.Serializable]
public class TilemapChunkData
{
    public string tilemapName;
    public int chunkX;
    public int chunkY;
    public List<TileData> tiles;
}
[System.Serializable]
public class ChunkTile
{
    public Vector3Int position;
    public string paletteName;
    public int tileIndexInPalette;
    public string layerName; // Название слоя в GridHierarchy (например, "ground")
}
[System.Serializable]
public class TileData
{
    public Vector3Int position;
    public string tileName;
}
[System.Serializable]
public class TilemapPaletteLink
{
    public string tilemapName; // имя Tilemap-а, например "Ground"
    public string paletteResourcePath; // путь в Resources к палитре
}