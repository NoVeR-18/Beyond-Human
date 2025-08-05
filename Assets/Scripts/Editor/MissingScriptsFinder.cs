using UnityEditor;
using UnityEngine;

public class MissingScriptsFinder : EditorWindow
{
    [MenuItem("Tools/Find Missing Scripts in Scene")]
    [System.Obsolete]
    static void FindMissingScripts()
    {
        int goCount = 0;
        int componentsCount = 0;
        int missingCount = 0;

        GameObject[] goArray = GameObject.FindObjectsOfType<GameObject>();
        foreach (GameObject g in goArray)
        {
            goCount++;
            Component[] components = g.GetComponents<Component>();

            for (int i = 0; i < components.Length; i++)
            {
                componentsCount++;
                if (components[i] == null)
                {
                    missingCount++;
                    Debug.Log($"Missing script found in: {GetFullPath(g)}", g);
                }
            }
        }

        Debug.Log($"Searched {goCount} GameObjects, {componentsCount} components, found {missingCount} missing.");
    }

    static string GetFullPath(GameObject obj)
    {
        return obj.transform.parent == null ? obj.name : GetFullPath(obj.transform.parent.gameObject) + "/" + obj.name;
    }
}
