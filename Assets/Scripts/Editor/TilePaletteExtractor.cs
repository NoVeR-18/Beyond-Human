#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class TilePaletteExtractor
{
    [MenuItem("Assets/������� TilePaletteData �� �������", true)]
    private static bool ValidatePalette()
    {
        return Selection.activeObject is GameObject;
    }

    [MenuItem("Assets/������� TilePaletteData �� �������")]
    private static void CreateTilePaletteData()
    {
        GameObject selected = Selection.activeObject as GameObject;

        if (selected == null)
        {
            Debug.LogError("��������� ������ �� �������� ��������.");
            return;
        }

        string path = AssetDatabase.GetAssetPath(selected);
        GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(selected);

        Tilemap tilemap = instance.GetComponentInChildren<Tilemap>();
        if (tilemap == null)
        {
            Debug.LogError("� ������� �� ������ Tilemap.");
            Object.DestroyImmediate(instance);
            return;
        }

        // �������� ��� ���������� �����
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

        // ������ TilePaletteData
        TilePaletteData paletteData = ScriptableObject.CreateInstance<TilePaletteData>();
        paletteData.tiles = new List<TileBase>(tiles);
        paletteData.paletteName = selected.name;
        // ��������� ����� � ��������
        string folder = Path.GetDirectoryName(path);
        string fileName = Path.GetFileNameWithoutExtension(path);
        string dataPath = Path.Combine(folder, fileName + "_PaletteData.asset");

        AssetDatabase.CreateAsset(paletteData, dataPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"������ TilePaletteData � {tiles.Count} ������� �� ����: {dataPath}");

        Object.DestroyImmediate(instance);
    }
}
#endif
