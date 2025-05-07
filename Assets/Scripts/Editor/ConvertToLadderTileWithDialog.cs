using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ConvertTileToLadderInlineEditor : EditorWindow
{
    private static string tilePath;
    private static string tileName;

    [MenuItem("Assets/Convert Tile To LadderTile (Replace in File)...", priority = 1000)]
    public static void ShowWindow()
    {
        Object selected = Selection.activeObject;

        if (!(selected is TileBase))
        {
            Debug.LogWarning("Please select a Tile asset.");
            return;
        }

        tilePath = AssetDatabase.GetAssetPath(selected);
        tileName = selected.name;

        ConvertTileToLadderInlineEditor window = CreateInstance<ConvertTileToLadderInlineEditor>();
        window.titleContent = new GUIContent("Ladder Direction");
        window.position = new Rect(Screen.width / 2, Screen.height / 2, 240, 170);
        window.ShowUtility();
    }

    void OnGUI()
    {
        GUILayout.Label($"Tile: {tileName}", EditorStyles.boldLabel);
        GUILayout.Label("Choose Stair Direction:");

        GUILayout.Space(8);

        GUILayout.BeginHorizontal();
        DrawDirectionButton("↖", new Vector2(-1, 1));
        DrawDirectionButton("↑", new Vector2(0, 1));
        DrawDirectionButton("↗", new Vector2(1, 1));
        GUILayout.EndHorizontal();

    }

    private void DrawDirectionButton(string label, Vector2 dir)
    {
        if (GUILayout.Button(label, GUILayout.Width(50), GUILayout.Height(30)))
        {
            ReplaceTileScriptWithLadderTile(tilePath, dir);
            Close();
        }
    }

    private void ReplaceTileScriptWithLadderTile(string path, Vector2 stairDirection)
    {
        string scriptPath = "Assets/Scripts/Save/LadderTile.cs";
        string scriptGUID = AssetDatabase.AssetPathToGUID(scriptPath);

        if (string.IsNullOrEmpty(scriptGUID))
        {
            Debug.LogError($"Could not find LadderTile.cs at: {scriptPath}");
            return;
        }

        string fileText = File.ReadAllText(path);

        // Заменим тип скрипта
        string newScriptLine = $"  m_Script: {{fileID: 11500000, guid: {scriptGUID}, type: 3}}";
        fileText = Regex.Replace(
            fileText,
            @"  m_Script: \{fileID: \d+, guid: [a-f0-9]+, type: \d+\}",
            newScriptLine
        );

        // Заменим имя объекта, чтобы не ругался на Main Object Name
        string filename = Path.GetFileNameWithoutExtension(path);
        fileText = Regex.Replace(fileText, @"  m_Name: .+", $"  m_Name: {filename}");

        // Добавим или заменим stairDirection
        if (fileText.Contains("stairDirection:"))
        {
            fileText = Regex.Replace(
                fileText,
                @"stairDirection: \{x: .*?, y: .*?\}",
                $"stairDirection: {{x: {stairDirection.x}, y: {stairDirection.y}}}"
            );
        }
        else
        {
            // Добавим после m_Script
            fileText = Regex.Replace(
                fileText,
                @"(  m_Script: .+\n)",
                $"$1  stairDirection: {{x: {stairDirection.x}, y: {stairDirection.y}}}\n"
            );
        }

        File.WriteAllText(path, fileText);
        AssetDatabase.Refresh();

        Debug.Log($"✅ Tile '{filename}' converted to LadderTile with direction {stairDirection}");

    }
}
