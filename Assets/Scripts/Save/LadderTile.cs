using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Tiles/LadderTile")]
public class LadderTile : Tile
{
    public Vector2 stairDirection = new Vector2(1, 1);
}