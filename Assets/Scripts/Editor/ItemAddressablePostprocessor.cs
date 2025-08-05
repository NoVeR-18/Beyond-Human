using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEngine;

public class ItemAddressablePostprocessor : AssetPostprocessor
{
    static void OnPostprocessAllAssets(
        string[] importedAssets,
        string[] deletedAssets,
        string[] movedAssets,
        string[] movedFromAssetPaths)
    {
        var settings = AddressableAssetSettingsDefaultObject.Settings;
        if (settings == null)
        {
            Debug.LogWarning("AddressableAssetSettings not found.");
            return;
        }

        foreach (string path in importedAssets)
        {
            if (!path.EndsWith(".asset")) continue;

            var item = AssetDatabase.LoadAssetAtPath<Item>(path);
            if (item == null) continue; // не является Item или наследником

            string guid = AssetDatabase.AssetPathToGUID(path);

            var entry = settings.FindAssetEntry(guid);
            if (entry == null)
            {
                // Если ещё не добавлен — добавляем
                var group = settings.DefaultGroup;
                entry = settings.CreateOrMoveEntry(guid, group);
            }

            entry.address = item.itemName;
            Debug.Log($" Addressable: {item.itemName} ({path})");
        }

        // Сохраняем
        AssetDatabase.SaveAssets();
    }
}
