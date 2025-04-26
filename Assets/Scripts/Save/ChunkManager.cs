using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

[ExecuteAlways]
public class ChunkManager : MonoBehaviour
{
    public List<Vector2Int> chunkPoss;
    public GridHierarchy gridHierarchy;
    public ChunkSaveSystem chunkSaveSystem;

    private HashSet<Vector2Int> dirtyChunks = new();

    private void OnEnable()
    {
        Tilemap.tilemapTileChanged += OnTileChanged;
    }

    private void OnDisable()
    {
        Tilemap.tilemapTileChanged -= OnTileChanged;
    }
    private void OnTileChanged(Tilemap tilemap, Tilemap.SyncTile[] changes)
    {
        if (tilemap == null)
            return;


        foreach (var change in changes)
        {
            Vector3Int cellPosition = change.position;
            Vector2Int chunkCoord = new(cellPosition.x / chunkSaveSystem.chunkSize, cellPosition.y / chunkSaveSystem.chunkSize);
            dirtyChunks.Add(chunkCoord);
            chunkPoss = dirtyChunks.ToList();
        }
    }


    [ContextMenu("Save Selected Chunks")]
    public void SaveChunk()
    {
        foreach (var chunkPos in chunkPoss)
        {
            chunkSaveSystem.SaveChunkAt(chunkPos);
        }
    }
    [ContextMenu("Remove Selected Chunks")]
    public void RemoveChunk()
    {
        foreach (var chunkPos in chunkPoss)
        {
            chunkSaveSystem.ClearChunk(chunkPos);
        }
    }

    [ContextMenu("Save All Chunks")]
    public void SaveAllChunks() => chunkSaveSystem.SaveAllChunks();

    [ContextMenu("Load All Chunks")]
    public void LoadAllChunks() => chunkSaveSystem.LoadAllChunks();

    [ContextMenu("Load Selected Chunks")]
    public void LoadChunk()
    {
        foreach (var chunkPos in chunkPoss)
        {
            chunkSaveSystem.LoadChunk(chunkPos);
        }
    }

    [ContextMenu("Save Dirty Chunks")]
    public void SaveDirtyChunks()
    {
        if (dirtyChunks.Count == 0)
        {
            Debug.Log("Нет изменённых чанков для сохранения.");
            return;
        }

        foreach (var chunkCoord in dirtyChunks)
        {
            chunkSaveSystem.SaveChunkAt(chunkCoord);
        }

        dirtyChunks.Clear();
        chunkPoss.Clear();
        Debug.Log("Все изменённые чанки успешно сохранены!");
    }
#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        var chunkSize = chunkSaveSystem.chunkSize;
        Handles.color = new Color(1, 1, 1, 0.3f);
        var tilemaps = gridHierarchy?.GetNamedTilemaps();
        if (tilemaps == null) return;

        HashSet<Vector2Int> seenChunks = new();
        foreach (var map in tilemaps.Values)
        {
            BoundsInt bounds = map.cellBounds;
            for (int x = bounds.xMin; x < bounds.xMax; x += chunkSize)
            {
                for (int y = bounds.yMin; y < bounds.yMax; y += chunkSize)
                {
                    Vector2Int chunk = new(x / chunkSize, y / chunkSize);
                    if (seenChunks.Contains(chunk)) continue;
                    seenChunks.Add(chunk);

                    Vector3 center = map.CellToWorld(new Vector3Int(chunk.x * chunkSize + chunkSize / 2, chunk.y * chunkSize + chunkSize / 2, 0));
                    Vector3 size = new(chunkSize, chunkSize, 0.1f);
                    Gizmos.DrawWireCube(center, size);
                    Handles.Label(center, $"({chunk.x},{chunk.y})");
                }
            }
        }
    }
#endif
}
