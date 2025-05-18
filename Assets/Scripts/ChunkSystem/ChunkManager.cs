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
        if (tilemap == null || changes == null || changes.Length == 0)
            return;

        HashSet<Vector2Int> changedChunks = new HashSet<Vector2Int>();

        foreach (var change in changes)
        {
            Vector3Int cellPosition = change.position;
            Vector2Int chunkCoord = new(
                Mathf.FloorToInt((float)cellPosition.x / chunkSaveSystem.chunkSize),
                Mathf.FloorToInt((float)cellPosition.y / chunkSaveSystem.chunkSize)
            );

            changedChunks.Add(chunkCoord);
        }

        foreach (var chunk in changedChunks)
        {
            dirtyChunks.Add(chunk);
        }

        chunkPoss = dirtyChunks.ToList();
    }


    [ContextMenu("Save Selected Chunks")]
    public void SaveChunk()
    {
        foreach (var chunkPos in chunkPoss)
        {
            chunkSaveSystem.SaveChunkAt(chunkPos);
        }
        dirtyChunks.Clear();
        chunkPoss.Clear();
    }
    [ContextMenu("Clear Selected Chunks")]
    public void ClearChunk()
    {
        foreach (var chunkPos in chunkPoss)
        {
            chunkSaveSystem.ClearChunk(chunkPos);
        }
        dirtyChunks.Clear();
        chunkPoss.Clear();
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

        dirtyChunks.Clear();
        chunkPoss.Clear();
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
