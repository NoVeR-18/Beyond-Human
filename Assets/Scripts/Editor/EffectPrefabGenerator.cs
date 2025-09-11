using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class EffectPrefabGenerator : EditorWindow
{
    private string folderPath = "Assets/Effects"; // папка с картинками

    [MenuItem("Tools/Generate Effect Prefabs")]
    public static void ShowWindow()
    {
        GetWindow(typeof(EffectPrefabGenerator));
    }

    void OnGUI()
    {
        GUILayout.Label("Effect Prefab Generator", EditorStyles.boldLabel);

        folderPath = EditorGUILayout.TextField("Folder Path", folderPath);

        if (GUILayout.Button("Generate Prefabs"))
        {
            GeneratePrefabs(folderPath);
        }
    }

    private static void GeneratePrefabs(string folderPath)
    {


        // ищем все png
        string[] files = Directory.GetFiles(folderPath, "*.png", SearchOption.AllDirectories);

        // папка для префабов
        foreach (string file in files)
        {
            string assetPath = file.Replace("\\", "/");
            Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);

            if (texture == null)
                continue;

            // загружаем все спрайты из ассета
            Object[] assets = AssetDatabase.LoadAllAssetsAtPath(assetPath);
            Sprite[] frames = assets.OfType<Sprite>().ToArray();

            if (frames.Length == 0)
                continue;

            // создаём GameObject
            GameObject effectGO = new GameObject(texture.name);
            SpriteRenderer sr = effectGO.AddComponent<SpriteRenderer>();

            EffectController controller = effectGO.AddComponent<EffectController>();
            controller.frames = frames;
            controller.frameRate = 0.05f;
            controller.sr = sr;


            string textureDir = Path.GetDirectoryName(assetPath).Replace("\\", "/");
            string prefabFolder = textureDir + "/GeneratedPrefabs";
            EnsureFolder(prefabFolder);
            // путь к префабу
            string prefabPath = prefabFolder + "/" + texture.name + ".prefab";

            // сохраняем
            PrefabUtility.SaveAsPrefabAsset(effectGO, prefabPath);

            Object.DestroyImmediate(effectGO);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
    private static void EnsureFolder(string path)
    {
        string[] parts = path.Split('/');
        string current = parts[0]; // "Assets"

        for (int i = 1; i < parts.Length; i++)
        {
            string next = parts[i];
            string combined = current + "/" + next;

            if (!AssetDatabase.IsValidFolder(combined))
            {
                AssetDatabase.CreateFolder(current, next);
            }

            current = combined;
        }
    }
}
