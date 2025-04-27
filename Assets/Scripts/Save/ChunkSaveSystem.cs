using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ChunkSaveSystem : MonoBehaviour
{
    public GridHierarchy gridHierarchy;
    public int chunkSize = 64;
    public string saveFolder = "Chunks";

    private HashSet<Vector2Int> loadedChunks = new();
    private Dictionary<string, TilePaletteData> paletteLookup = new();
    [SerializeField]
    private List<TilePaletteData> palettes = new();

    private void LoadPalettesFromResources()
    {
        if (palettes == null)
        {
            paletteLookup.Clear();
            TilePaletteData[] palettes = Resources.LoadAll<TilePaletteData>("TilePalettes");
            foreach (var palette in palettes)
            {
                paletteLookup[palette.paletteName] = palette;
            }
        }
        else
        {
            foreach (var palette in palettes)
            {
                if (!paletteLookup.ContainsKey(palette.paletteName))
                {
                    paletteLookup[palette.paletteName] = palette;
                }
            }
        }


    }
    public void SaveChunkAt(Vector2Int chunkCoord)
    {
        LoadPalettesFromResources();
        var tilemaps = gridHierarchy.GetNamedTilemaps();

        List<ChunkData> tiles = new();

        foreach (var kvp in tilemaps)
        {
            string layerName = kvp.Key;
            Tilemap map = kvp.Value;
            Vector3Int offset = Vector3Int.zero;
            if (gridHierarchy.tilemapOffsets.TryGetValue(layerName, out var foundOffset))
            {
                offset = foundOffset;
            }

            BoundsInt bounds = map.cellBounds;

            for (int x = bounds.xMin; x < bounds.xMax; x++)
            {
                for (int y = bounds.yMin; y < bounds.yMax; y++)
                {
                    // —читаем мировые координаты тайла
                    int worldX = x + offset.x;
                    int worldY = y + offset.y;

                    int cx = Mathf.FloorToInt((float)worldX / chunkSize);
                    int cy = Mathf.FloorToInt((float)worldY / chunkSize);

                    if (cx != chunkCoord.x || cy != chunkCoord.y) continue;

                    Vector3Int pos = new(x, y, 0);
                    TileBase tile = map.GetTile(pos);
                    if (tile == null) continue;

                    foreach (var palette in paletteLookup.Values)
                    {
                        int index = palette.tiles.IndexOf(tile);
                        if (index >= 0)
                        {
                            tiles.Add(new ChunkData
                            {
                                position = new Vector3Int(worldX, worldY, 0),
                                paletteName = palette.paletteName,
                                tileIndexInPalette = index,
                                layerName = layerName
                            });
                            break;
                        }
                    }
                }
            }
        }

        string path = Path.Combine(saveFolder, $"chunk_{chunkCoord.x}_{chunkCoord.y}.dat");
        Debug.Log($"Save chunk {chunkCoord}");
        Directory.CreateDirectory(saveFolder);
        using (BinaryWriter writer = new(File.Open(path, FileMode.Create)))
        {
            writer.Write(tiles.Count);
            foreach (var tile in tiles)
            {
                writer.Write(tile.position.x);
                writer.Write(tile.position.y);
                writer.Write(tile.layerName);
                writer.Write(tile.paletteName);
                writer.Write(tile.tileIndexInPalette);
            }
        }
    }


    public void LoadChunk(Vector2Int chunkPos)
    {
        LoadPalettesFromResources();
        var tilemaps = gridHierarchy.GetNamedTilemaps();

        string path = Path.Combine(saveFolder, $"chunk_{chunkPos.x}_{chunkPos.y}.dat");
        if (!File.Exists(path))
        {
            Debug.LogWarning($"Chunk not found: {chunkPos}");
            return;
        }

        List<ChunkData> tiles = new();

        using (BinaryReader reader = new(File.Open(path, FileMode.Open)))
        {
            int count = reader.ReadInt32();
            Debug.Log($"Load chunk {chunkPos}");

            for (int i = 0; i < count; i++)
            {
                int x = reader.ReadInt32();
                int y = reader.ReadInt32();
                string layerName = reader.ReadString();
                string paletteName = reader.ReadString();
                int tileIndex = reader.ReadInt32();

                tiles.Add(new ChunkData
                {
                    position = new Vector3Int(x, y, 0),
                    layerName = layerName,
                    paletteName = paletteName,
                    tileIndexInPalette = tileIndex
                });
            }
        }
#if UNITY_EDITOR
        // --- Undo до начала изменений
        foreach (var map in tilemaps.Values)
        {
            UnityEditor.Undo.RegisterCompleteObjectUndo(map, "Load Chunk");
        }
#endif
        foreach (var tile in tiles)
        {
            if (!tilemaps.ContainsKey(tile.layerName)) continue;
            if (!paletteLookup.ContainsKey(tile.paletteName)) continue;

            Tilemap map = tilemaps[tile.layerName];
            Vector3Int offset = Vector3Int.zero;
            if (gridHierarchy.tilemapOffsets.TryGetValue(tile.layerName, out var foundOffset))
            {
                offset = foundOffset;
            }

            TileBase tileAsset = paletteLookup[tile.paletteName].tiles[tile.tileIndexInPalette];
            map.SetTile(tile.position - offset, tileAsset);
        }

        loadedChunks.Add(chunkPos);
    }

    public void LoadChunksAround(Vector2 worldPos, int radius)
    {
        Vector2Int centerChunk = WorldToChunk(worldPos);

        for (int x = -radius; x <= radius; x++)
        {
            for (int y = -radius; y <= radius; y++)
            {
                Vector2Int pos = new(centerChunk.x + x, centerChunk.y + y);
                if (!loadedChunks.Contains(pos))
                {
                    LoadChunk(pos);

                    // ѕроверка Ч был ли загружен файл
                    string path = Path.Combine(saveFolder, $"chunk_{pos.x}_{pos.y}.dat");
                    if (!File.Exists(path))
                    {
                        Debug.LogWarning($"File of chunk not found: {path}");
                    }
                    else
                    {
                        Debug.Log($"Chunk load: {pos}");
                    }
                }
            }
        }
    }


    public void UnloadChunksOutside(Vector2 worldPos, int radius)
    {
        Vector2Int center = WorldToChunk(worldPos);
        var toRemove = new List<Vector2Int>();
        foreach (var chunk in loadedChunks)
        {
            if (Mathf.Abs(chunk.x - center.x) > radius || Mathf.Abs(chunk.y - center.y) > radius)
            {
                toRemove.Add(chunk);
            }
        }

        foreach (var chunk in toRemove)
        {
            loadedChunks.Remove(chunk);
            ClearChunk(chunk);
        }
    }

    public void ClearChunk(Vector2Int chunk)
    {
        var tilemaps = gridHierarchy.GetNamedTilemaps();

        foreach (var kvp in tilemaps)
        {
            string layerName = kvp.Key;
            Tilemap map = kvp.Value;
            UnityEditor.Undo.RegisterCompleteObjectUndo(map, "Clear Chunk");
            Vector3Int offset = Vector3Int.zero;
            if (gridHierarchy.tilemapOffsets.TryGetValue(layerName, out var foundOffset))
            {
                offset = foundOffset;
            }

            // —читаем границы чанка с учЄтом оффсета
            int startX = chunk.x * chunkSize - offset.x;
            int startY = chunk.y * chunkSize - offset.y;

            BoundsInt bounds = new(startX, startY, 0, chunkSize, chunkSize, 1);

            for (int x = bounds.xMin; x < bounds.xMax; x++)
            {
                for (int y = bounds.yMin; y < bounds.yMax; y++)
                {
                    map.SetTile(new Vector3Int(x, y, 0), null);
                }
            }
        }
    }


    private Vector2Int WorldToChunk(Vector2 pos)
    {
        Vector3Int tile = gridHierarchy.ground.WorldToCell(pos);
        int cx = Mathf.FloorToInt((float)tile.x / chunkSize);
        int cy = Mathf.FloorToInt((float)tile.y / chunkSize);
        return new Vector2Int(cx, cy);
    }
    public void SaveAllChunks()
    {
        LoadPalettesFromResources();
        var tilemaps = gridHierarchy.GetNamedTilemaps();
        HashSet<Vector2Int> foundChunks = new();

        foreach (var kvp in tilemaps)
        {
            Tilemap map = kvp.Value;
            BoundsInt bounds = map.cellBounds;

            for (int x = bounds.xMin; x < bounds.xMax; x++)
            {
                for (int y = bounds.yMin; y < bounds.yMax; y++)
                {
                    Vector3Int pos = new(x, y, 0);
                    TileBase tile = map.GetTile(pos);
                    if (tile == null) continue;

                    Vector2Int chunk = new(x / chunkSize, y / chunkSize);
                    if (!foundChunks.Contains(chunk))
                    {
                        foundChunks.Add(chunk);
                        SaveChunkAt(chunk);
                    }
                }
            }
        }
    }
    public void LoadAllChunks()
    {
        if (!Directory.Exists(saveFolder)) return;

        string[] files = Directory.GetFiles(saveFolder, "chunk_*.dat");

        foreach (string file in files)
        {
            string fileName = Path.GetFileNameWithoutExtension(file);
            string[] parts = fileName.Split('_');
            if (parts.Length == 3 &&
                int.TryParse(parts[1], out int x) &&
                int.TryParse(parts[2], out int y))
            {
                Vector2Int chunkCoord = new Vector2Int(x, y);
                LoadChunk(chunkCoord);
            }
        }
    }
}
