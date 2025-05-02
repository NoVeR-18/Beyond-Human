using NavMeshPlus.Components;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

public class ItemPrefabGenerator
{
    [MenuItem("Tools/Generate Item Prefabs")]
    static void GenerateItemPrefabs()
    {
        string rootPath = "Assets/Items/";
        string[] itemFolders = Directory.GetDirectories(rootPath, "*", SearchOption.AllDirectories);

        foreach (string folder in itemFolders)
        {
            string[] pngFiles = Directory.GetFiles(folder, "*.png");

            foreach (string pngPath in pngFiles)
            {
                string assetPath = pngPath.Replace("\\", "/");
                string fileName = Path.GetFileNameWithoutExtension(assetPath);
                string folderPath = Path.GetDirectoryName(assetPath).Replace("\\", "/") + "/";
                string prefabPath = folderPath + fileName + ".prefab";

                if (File.Exists(prefabPath))
                {
                    Debug.Log($"[SKIP] Prefab already exists: {prefabPath}");
                    continue;
                }

                Object[] loadedAssets = AssetDatabase.LoadAllAssetsAtPath(assetPath);
                Sprite[] sprites = loadedAssets.OfType<Sprite>().OrderBy(s => s.name).ToArray();

                if (sprites.Length == 0)
                {
                    Debug.LogWarning($"[WARNING] No sprites found at {assetPath}");
                    continue;
                }

                GameObject go = new GameObject(fileName);
                var sr = go.AddComponent<SpriteRenderer>();
                sr.sprite = sprites[0];

                var collider = go.AddComponent<BoxCollider2D>();
                if (sprites[0] != null)
                {
                    collider.size = sprites[0].bounds.size;
                    collider.offset = sprites[0].bounds.center;
                }

                var navMod = go.AddComponent<NavMeshModifierVolume>();
                navMod.size = new Vector3(1f, 1f, 1f);
                navMod.area = 1;

                go.AddComponent<DecorObject>();

                if (sprites.Length > 1)
                {
                    AnimationClip clip = CreateAnimationClip(sprites, fileName, folderPath);

                    Animator animator = go.AddComponent<Animator>();
                    string controllerPath = folderPath + fileName + "_controller.controller";

                    AnimatorController controller = AnimatorController.CreateAnimatorControllerAtPath(controllerPath);
                    controller.AddMotion(clip);
                    animator.runtimeAnimatorController = controller;

                    Debug.Log($"[OK] Created animated item prefab: {fileName}");
                }
                else
                {
                    Debug.Log($"[OK] Created static item prefab: {fileName}");
                }

                PrefabUtility.SaveAsPrefabAsset(go, prefabPath);
                GameObject.DestroyImmediate(go);
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("<color=green>All item prefabs generated!</color>");
    }

    static AnimationClip CreateAnimationClip(Sprite[] sprites, string baseName, string savePath)
    {
        AnimationClip clip = new AnimationClip();
        clip.frameRate = 10f;

        EditorCurveBinding spriteBinding = new EditorCurveBinding
        {
            type = typeof(SpriteRenderer),
            path = "",
            propertyName = "m_Sprite"
        };

        ObjectReferenceKeyframe[] keyFrames = new ObjectReferenceKeyframe[sprites.Length];
        for (int i = 0; i < sprites.Length; i++)
        {
            keyFrames[i] = new ObjectReferenceKeyframe
            {
                time = i / 10f,
                value = sprites[i]
            };
        }

        AnimationUtility.SetObjectReferenceCurve(clip, spriteBinding, keyFrames);

        string clipPath = $"{savePath}{baseName}_anim.anim";
        AssetDatabase.CreateAsset(clip, clipPath);
        return clip;
    }
}
