using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CreateAnimatedTileFromSelection
{
    [MenuItem("Assets/Animated Tile From Sprites", true)]
    private static bool ValidateCreateTile()
    {
        return Selection.objects.All(o => o is Sprite);
    }

    [MenuItem("Assets/Animated Tile From Sprites")]
    private static void CreateTile()
    {
        Object[] selected = Selection.objects;
        var sprites = selected.OfType<Sprite>().OrderBy(s => s.name).ToArray();

        if (sprites.Length < 2)
        {
            EditorUtility.DisplayDialog("Error", "Select at least 2 sprites to create an animated tile.", "OK");
            return;
        }

        string path = AssetDatabase.GetAssetPath(sprites[0]);
        string directory = Path.GetDirectoryName(path);
        string name = Path.GetFileNameWithoutExtension(path).Split('_')[0];

        string savePath = EditorUtility.SaveFilePanelInProject(
            "Save Animated Tile",
            $"{name}_Animated.asset",
            "asset",
            "Save Animated Tile in project",
            directory
        );

        if (string.IsNullOrEmpty(savePath)) return;

        AnimatedTile tile = ScriptableObject.CreateInstance<AnimatedTile>();
        tile.m_AnimatedSprites = sprites;
        tile.m_MinSpeed = 5f;
        tile.m_MaxSpeed = 5f;
        tile.m_TileColliderType = Tile.ColliderType.None;

        AssetDatabase.CreateAsset(tile, savePath);
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();
        Selection.activeObject = tile;

        Debug.Log($"[OK] Animated tile created: {savePath}");
    }
}
