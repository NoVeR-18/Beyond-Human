using System.IO;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEngine;

public class AssignBattlePrefabs : Editor
{
    [MenuItem("Tools/Assign Battle Prefabs")]
    public static void AssignPrefabs()
    {
        string targetFolder = "Assets/BattleData/Prefabs";
        string[] guids = AssetDatabase.FindAssets("t:Prefab", new[] { targetFolder });

        var settings = AddressableAssetSettingsDefaultObject.Settings;
        if (settings == null)
        {
            Debug.LogError("Addressable Asset Settings не найдены. Создайте их через Window → Asset Management → Addressables → Groups.");
            return;
        }

        // Ищем или создаём группу "BattlePrefabs"
        var group = settings.FindGroup("BattlePrefabs");
        if (group == null)
        {
            group = settings.CreateGroup("BattlePrefabs", false, false, false, null, typeof(BundledAssetGroupSchema));
        }

        int count = 0;

        foreach (var guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            string fileName = Path.GetFileName(path); // Имя с .prefab

            var entry = settings.FindAssetEntry(guid);
            if (entry == null)
            {
                entry = settings.CreateOrMoveEntry(guid, group);
            }
            else
            {
                entry.SetLabel("BattlePrefab", true); // Добавляем метку "BattlePrefab"
            }

            entry.address = fileName; // Имя + .prefab

            count++;
        }

        AssetDatabase.SaveAssets();
        Debug.Log($"Назначено {count} префабов в группу 'BattlePrefabs'.");
    }
}
