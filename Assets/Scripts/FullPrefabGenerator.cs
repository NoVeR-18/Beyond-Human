using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

public class FullSmartPrefabGenerator : MonoBehaviour
{
    private static List<string> logLines = new List<string>(); // Сбор логов для записи в файл

    [MenuItem("Tools/Generate Strict Character Prefabs")]
    static void GeneratePrefabs()
    {
        string rootSpritesPath = "Assets/Сharacters/"; // Путь к папкам со спрайтами
        string logFilePath = "Assets/PrefabGenerationLog.txt"; // Путь к файлу логов

        string[] characterFolders = Directory.GetDirectories(rootSpritesPath);

        foreach (var folder in characterFolders)
        {
            string characterName = Path.GetFileName(folder);
            string relativeFolderPath = folder.Replace("\\", "/") + "/";
            string prefabPath = relativeFolderPath + characterName + ".prefab";

            // Проверяем: есть ли уже префаб?
            if (File.Exists(prefabPath))
            {
                LogLine($"[SKIPPED] Prefab already exists for {characterName}, skipping.");
                continue;
            }

            string[] spriteFiles = Directory.GetFiles(folder, "*.png");

            if (spriteFiles.Length == 0)
            {
                LogLine($"[WARNING] No sprite files found in {characterName} folder.");
                continue;
            }

            // Сортируем файлы по имени
            spriteFiles = spriteFiles.OrderBy(f => f).ToArray();

            List<Sprite> walkSprites = new List<Sprite>();
            List<Sprite> idleSprites = new List<Sprite>();

            foreach (var file in spriteFiles)
            {
                // Загружаем все вложенные ассеты
                Object[] allAssets = AssetDatabase.LoadAllAssetsAtPath(file.Replace("\\", "/"));
                foreach (var asset in allAssets)
                {
                    if (asset is Sprite sprite)
                    {
                        string lowerSpriteName = sprite.name.ToLower();

                        if (lowerSpriteName.Contains("walk"))
                            walkSprites.Add(sprite);
                        else if (lowerSpriteName.Contains("idle"))
                            idleSprites.Add(sprite);
                    }
                }
            }

            // Проверка: если нет вообще ни одной группы спрайтов, пропускаем
            if (walkSprites.Count == 0 && idleSprites.Count == 0)
            {
                LogLine($"[WARNING] Character '{characterName}' skipped — missing both required animations (Walk & Idle).");
                continue;
            }

            // Создаем GameObject персонажа
            GameObject characterGO = new GameObject(characterName);
            var spriteRenderer = characterGO.AddComponent<SpriteRenderer>();

            // Выставляем дефолтный спрайт
            if (idleSprites.Count > 0)
                spriteRenderer.sprite = idleSprites[0];
            else if (walkSprites.Count > 0)
                spriteRenderer.sprite = walkSprites[0];

            // Создаем AnimatorController
            string controllerPath = relativeFolderPath + characterName + "_Controller.controller";
            AnimatorController animatorController = AnimatorController.CreateAnimatorControllerAtPath(controllerPath);

            AnimatorStateMachine sm = animatorController.layers[0].stateMachine;

            // Добавляем параметр Speed
            animatorController.AddParameter("Speed", AnimatorControllerParameterType.Float);

            AnimationClip walkClip = null;
            AnimationClip idleClip = null;

            if (walkSprites.Count > 0)
                walkClip = CreateAnimationClip(walkSprites, "Walk", relativeFolderPath);

            if (idleSprites.Count > 0)
                idleClip = CreateAnimationClip(idleSprites, "Idle", relativeFolderPath);

            // Проверка безопасности
            if (walkClip == null && idleClip == null)
            {
                LogLine($"[ERROR] Character '{characterName}' skipped — failed to create any animation clips.");
                GameObject.DestroyImmediate(characterGO);
                continue;
            }

            AnimatorState walkState = null;
            AnimatorState idleState = null;

            if (idleClip != null)
            {
                idleState = sm.AddState("Idle");
                idleState.motion = idleClip;
                sm.defaultState = idleState;
            }

            if (walkClip != null)
            {
                walkState = sm.AddState("Walk");
                walkState.motion = walkClip;

                if (idleState != null)
                {
                    // Делаем переходы только если обе анимации есть
                    AnimatorStateTransition idleToWalk = idleState.AddTransition(walkState);
                    idleToWalk.AddCondition(AnimatorConditionMode.Greater, 0.1f, "Speed");
                    idleToWalk.hasExitTime = false;

                    AnimatorStateTransition walkToIdle = walkState.AddTransition(idleState);
                    walkToIdle.AddCondition(AnimatorConditionMode.Less, 0.1f, "Speed");
                    walkToIdle.hasExitTime = false;
                }
                else
                {
                    // Если Idle нету — Walk становится дефолтным состоянием
                    sm.defaultState = walkState;
                }
            }

            // Назначаем Animator
            var animator = characterGO.AddComponent<Animator>();
            animator.runtimeAnimatorController = animatorController;

            // Прикрепляем скрипты EnemyAI и NPCRender
            characterGO.AddComponent<EnemyAI>();
            characterGO.AddComponent<NPCRender>();

            // Сохраняем префаб
            PrefabUtility.SaveAsPrefabAsset(characterGO, prefabPath);

            GameObject.DestroyImmediate(characterGO);

            LogLine($"<color=green>[SUCCESS]</color> Character prefab '{characterName}' created successfully!");
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        // Сохраняем все логи в файл
        File.WriteAllLines(logFilePath, logLines);

        Debug.Log($"<color=lime>All valid character prefabs generated!</color> Логи сохранены в файл: {logFilePath}");
    }

    static AnimationClip CreateAnimationClip(List<Sprite> sprites, string animationName, string saveFolder)
    {
        AnimationClip clip = new AnimationClip();
        EditorCurveBinding spriteBinding = new EditorCurveBinding
        {
            type = typeof(SpriteRenderer),
            path = "",
            propertyName = "m_Sprite"
        };

        ObjectReferenceKeyframe[] keyFrames = new ObjectReferenceKeyframe[sprites.Count];
        for (int i = 0; i < sprites.Count; i++)
        {
            keyFrames[i] = new ObjectReferenceKeyframe
            {
                time = i * (1f / 10f), // 10 кадров в секунду
                value = sprites[i]
            };
        }

        AnimationUtility.SetObjectReferenceCurve(clip, spriteBinding, keyFrames);

        AssetDatabase.CreateAsset(clip, $"{saveFolder}{animationName}.anim");
        return clip;
    }

    static void LogLine(string message)
    {
        Debug.Log(message);
        logLines.Add(message);
    }
}
