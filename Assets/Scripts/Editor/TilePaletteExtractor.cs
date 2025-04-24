#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class TilePaletteExtractor
{
    [MenuItem("Assets/Создать TilePaletteData из палитры", true)]
    private static bool ValidatePalette()
    {
        return Selection.activeObject is GameObject;
    }

    [MenuItem("Assets/Создать TilePaletteData из палитры")]
    private static void CreateTilePaletteData()
    {
        GameObject selected = Selection.activeObject as GameObject;

        if (selected == null)
        {
            Debug.LogError("Выбранный объект не является префабом.");
            return;
        }

        string path = AssetDatabase.GetAssetPath(selected);
        GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(selected);

        Tilemap tilemap = instance.GetComponentInChildren<Tilemap>();
        if (tilemap == null)
        {
            Debug.LogError("В префабе не найден Tilemap.");
            Object.DestroyImmediate(instance);
            return;
        }

        // Собираем все уникальные тайлы
        var tiles = new HashSet<TileBase>();
        BoundsInt bounds = tilemap.cellBounds;

        for (int x = bounds.xMin; x < bounds.xMax; x++)
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int pos = new(x, y, 0);
                TileBase tile = tilemap.GetTile(pos);
                if (tile != null)
                    tiles.Add(tile);
            }

        // Создаём TilePaletteData
        TilePaletteData paletteData = ScriptableObject.CreateInstance<TilePaletteData>();
        paletteData.tiles = new List<TileBase>(tiles);
        paletteData.paletteName = selected.name;
        // Сохраняем рядом с префабом
        string folder = Path.GetDirectoryName(path);
        string fileName = Path.GetFileNameWithoutExtension(path);
        string dataPath = Path.Combine(folder, fileName + "_PaletteData.asset");

        AssetDatabase.CreateAsset(paletteData, dataPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"Создан TilePaletteData с {tiles.Count} тайлами по пути: {dataPath}");

        Object.DestroyImmediate(instance);
    }
}
#endif
