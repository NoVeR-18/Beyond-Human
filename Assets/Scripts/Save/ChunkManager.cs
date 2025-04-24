using UnityEngine;


public class ChunkManager : MonoBehaviour
{
    public string chunkName = "chunk_0_0";
    public GridHierarchy gridHierarchy;
    public ChunkSaveSystem chunkSaveSystem;
    [ContextMenu("Save Chunk")]
    public void SaveChunk() => chunkSaveSystem.SaveAllChunks();

    [ContextMenu("Load Chunk")]
    public void LoadChunk() => chunkSaveSystem.LoadChunk(new Vector2Int(0, 0));
}