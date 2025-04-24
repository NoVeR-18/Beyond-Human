using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ChunkSaveSystem : MonoBehaviour
{
    public GridHierarchy gridHierarchy;
    public int chunkSize = 64;
    public string saveFolder = "Chunks";

    private Dictionary<string, TilePaletteData> paletteLookup = new();

    private void LoadPalettesFromResources()
    {
        TilePaletteData[] palettes = Resources.LoadAll<TilePaletteData>("");
        foreach (var palette in palettes)
        {
            paletteLookup[palette.paletteName] = palette;
        }
    }

    public void SaveAllChunks()
    {
        LoadPalettesFromResources();
        var tilemaps = gridHierarchy.GetNamedTilemaps();

        Dictionary<Vector2Int, ChunkData> allChunks = new();

        foreach (var kvp in tilemaps)
        {
            string layerName = kvp.Key;
            Tilemap map = kvp.Value;
            BoundsInt bounds = map.cellBounds;

            for (int x = bounds.xMin; x < bounds.xMax; x++)
            {
                for (int y = bounds.yMin; y < bounds.yMax; y++)
                {
                    Vector3Int pos = new(x, y, 0);
                    TileBase tile = map.GetTile(pos);
                    if (tile == null) continue;

                    string paletteName = null;
                    int tileIndex = -1;

                    foreach (var palette in paletteLookup.Values)
                    {
                        int index = palette.tiles.IndexOf(tile);
                        if (index >= 0)
                        {
                            paletteName = palette.paletteName;
                            tileIndex = index;
                            break;
                        }
                    }

                    if (paletteName == null) continue;

                    Vector2Int chunkPos = new(x / chunkSize, y / chunkSize);
                    if (!allChunks.ContainsKey(chunkPos))
                        allChunks[chunkPos] = new ChunkData { chunkPosition = chunkPos };

                    allChunks[chunkPos].tiles.Add(new ChunkTile
                    {
                        position = pos,
                        paletteName = paletteName,
                        tileIndexInPalette = tileIndex,
                        layerName = layerName
                    });
                }
            }
        }

        if (!Directory.Exists(saveFolder)) Directory.CreateDirectory(saveFolder);

        foreach (var chunk in allChunks)
        {
            string path = Path.Combine(saveFolder, $"chunk_{chunk.Key.x}_{chunk.Key.y}.json");
            File.WriteAllText(path, JsonUtility.ToJson(chunk.Value));
        }
    }

    public void LoadChunk(Vector2Int chunkPos)
    {
        LoadPalettesFromResources();
        var tilemaps = gridHierarchy.GetNamedTilemaps();

        string path = Path.Combine(saveFolder, $"chunk_{chunkPos.x}_{chunkPos.y}.json");
        if (!File.Exists(path)) return;

        ChunkData chunk = JsonUtility.FromJson<ChunkData>(File.ReadAllText(path));

        foreach (var chunkTile in chunk.tiles)
        {
            if (!tilemaps.ContainsKey(chunkTile.layerName)) continue;
            if (!paletteLookup.ContainsKey(chunkTile.paletteName)) continue;

            Tilemap map = tilemaps[chunkTile.layerName];
            TileBase tile = paletteLookup[chunkTile.paletteName].tiles[chunkTile.tileIndexInPalette];

            map.SetTile(chunkTile.position, tile);
        }
    }
}