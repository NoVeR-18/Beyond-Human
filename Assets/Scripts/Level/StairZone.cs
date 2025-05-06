using UnityEngine;
using UnityEngine.Tilemaps;

public class StairZone : MonoBehaviour
{
    public Tilemap ladderTilemap;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerController player = other.GetComponent<PlayerController>();
        if (player == null) return;

        Vector3 worldPos = other.transform.position;
        Vector3Int cellPos = ladderTilemap.WorldToCell(worldPos);
        TileBase tile = ladderTilemap.GetTile(cellPos);

        if (tile is LadderTile ladder)
        {
            player.SetOnStairs(true, 0f, 1f, ladder.stairDirection);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerController player = other.GetComponent<PlayerController>();
        if (player == null) return;

        player.SetOnStairs(false, 0f, 0f, Vector2.zero);
    }
}
