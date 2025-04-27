using UnityEngine;

public class ChunkStreamer : MonoBehaviour
{
    public ChunkSaveSystem chunkSystem;
    public int radius = 2;

    private Vector2Int lastChunkPos = Vector2Int.zero;
    private void Start()
    {
        Vector2Int lastChunkPos = WorldToChunk(transform.position);
        chunkSystem.LoadChunksAround(transform.position, radius);
    }
    void LateUpdate()
    {
        Vector2Int currentChunk = WorldToChunk(transform.position);

        if (currentChunk != lastChunkPos)
        {
            lastChunkPos = currentChunk;
            chunkSystem.LoadChunksAround(transform.position, radius);
            chunkSystem.UnloadChunksOutside(transform.position, radius);
        }
    }

    private Vector2Int WorldToChunk(Vector2 pos)
    {
        Vector3Int tile = chunkSystem.gridHierarchy.ground.WorldToCell(pos);
        int cx = Mathf.FloorToInt((float)tile.x / chunkSystem.chunkSize);
        int cy = Mathf.FloorToInt((float)tile.y / chunkSystem.chunkSize);
        return new Vector2Int(cx, cy);
    }
}
