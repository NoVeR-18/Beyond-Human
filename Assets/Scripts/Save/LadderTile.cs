using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Tiles/LadderTile")]
public class LadderTile : Tile
{
    public Vector2 stairDirection = new Vector2(1, 1);
}
public class ConvertToLadderTileWithDialog : EditorWindow
{
    private static Tile selectedTile;
    private Vector2 stairDirection = new Vector2(1, 1);

    [MenuItem("Assets/Convert to LadderTile (With Dialog)", true)]
    private static bool Validate()
    {
        return Selection.activeObject is Tile;
    }

    [MenuItem("Assets/Convert to LadderTile (With Dialog)")]
    private static void Init()
    {
        selectedTile = Selection.activeObject as Tile;
        if (selectedTile == null)
        {
            Debug.LogWarning("Selected asset is not a Tile.");
            return;
        }

        var window = GetWindow<ConvertToLadderTileWithDialog>("Convert to LadderTile");
        window.position = new Rect(Screen.width / 2, Screen.height / 2, 300, 120);
    }

    void OnGUI()
    {
        if (selectedTile == null)
        {
            EditorGUILayout.HelpBox("No Tile selected. Please close and re-run from Assets menu.", MessageType.Error);
            return;
        }

        EditorGUILayout.LabelField("Convert Tile to LadderTile", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("Original tile:", selectedTile.name);
        stairDirection = EditorGUILayout.Vector2Field("Stair Direction", stairDirection);

        GUILayout.Space(10);

        if (GUILayout.Button("Convert"))
        {
            ReplaceTile();
            Close();
        }
    }
    private void ReplaceTile()
    {
        string path = AssetDatabase.GetAssetPath(selectedTile);

        // Загружаем свойства
        Sprite sprite = selectedTile.sprite;
        Color color = selectedTile.color;
        Tile.ColliderType collider = selectedTile.colliderType;

        // Удаляем объект из памяти, не трогая файл
        ScriptableObject.DestroyImmediate(selectedTile, true);

        // Создаём новый LadderTile
        LadderTile newTile = ScriptableObject.CreateInstance<LadderTile>();
        newTile.sprite = sprite;
        newTile.color = color;
        newTile.colliderType = collider;
        newTile.stairDirection = stairDirection;

        AssetDatabase.CreateAsset(newTile, path);
        Undo.RegisterCreatedObjectUndo(newTile, "Converted to LadderTile");

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorUtility.FocusProjectWindow();
        Selection.activeObject = newTile;

        Debug.Log($"Tile converted to LadderTile at {path}, direction {stairDirection}");
    }

}
