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

        public static string PartyFile => Path.Combine(SavePath, "party.json");
        public static string InventoryFile => Path.Combine(SavePath, "inventory.json");

        public static void EnsureDirectory()
        {
            if (!Directory.Exists(SavePath))
                Directory.CreateDirectory(SavePath);
        }
    }
}
