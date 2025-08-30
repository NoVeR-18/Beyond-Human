using System.IO;
using UnityEngine;

namespace GameUtils.Utils
{

    public static class Utils
    {
        public static Vector3 GetRandomDir()
        {
            return new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
        }

    }

    public static class SaveUtils
    {
        public static string SavePath => Path.Combine(Application.persistentDataPath, "save");
        public static string QuestsFile => Path.Combine(SavePath, "quests.json");

        public static string PartyFile => Path.Combine(SavePath, "party.json");
        public static string InventoryFile => Path.Combine(SavePath, "inventory.json");

        public static string NPCFile => Path.Combine(Application.persistentDataPath, "npc_save.json");

        public static void EnsureDirectory()
        {
            if (!Directory.Exists(SavePath))
                Directory.CreateDirectory(SavePath);
        }
    }
    public static class AddressablesUtility
    {
        public static string GetAddressableKey(GameObject obj)
        {
#if UNITY_EDITOR
            var settings = UnityEditor.AddressableAssets.AddressableAssetSettingsDefaultObject.Settings;
            var guid = UnityEditor.AssetDatabase.AssetPathToGUID(UnityEditor.AssetDatabase.GetAssetPath(obj));
            var entry = settings.FindAssetEntry(guid);
            return entry != null ? entry.address : null;
#else
        return null; // В рантайме ключ уже должен быть сохранён в save-файле
#endif
        }
    }
}
