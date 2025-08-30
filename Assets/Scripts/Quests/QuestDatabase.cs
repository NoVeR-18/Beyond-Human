using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "QuestDatabase", menuName = "Quests/Quest Database")]
public class QuestDatabase : ScriptableObject
{
    public List<QuestData> quests = new List<QuestData>();

#if UNITY_EDITOR
    [ContextMenu("AutoFill")]
    public void AutoFill()
    {
        quests.Clear();

        string[] guids = AssetDatabase.FindAssets("t:QuestData");
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            QuestData quest = AssetDatabase.LoadAssetAtPath<QuestData>(path);
            if (quest != null)
                quests.Add(quest);
        }

        EditorUtility.SetDirty(this);
        Debug.Log($"QuestDatabase fill ({quests.Count} quests find)");
    }
#endif
}
