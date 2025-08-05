#if UNITY_EDITOR
using Assets.Scripts.NPC;
using System;
using UnityEditor;
using UnityEngine;

public static class NPCIdBatchGenerator
{
    [MenuItem("Tools/Generate NPC IDs in Scene")]
    [Obsolete]
    public static void GenerateIds()
    {
        var npcs = GameObject.FindObjectsOfType<NPCController>();
        int count = 0;

        foreach (var npc in npcs)
        {
            var so = new SerializedObject(npc);
            var prop = so.FindProperty("npcId");
            if (string.IsNullOrEmpty(prop.stringValue))
            {
                string newId = $"{npc.gameObject.scene.name}_{npc.gameObject.name}_{Guid.NewGuid().ToString().Substring(0, 8)}";
                prop.stringValue = newId;
                so.ApplyModifiedProperties();
                EditorUtility.SetDirty(npc);
                count++;
            }
        }

        Debug.Log($"[NPC ID Generator] Сгенерировано ID для {count} NPC");
    }
    [MenuItem("Tools/Generate NavTarget IDs in Scene")]
    [Obsolete]
    public static void GenerateNavTargetPoinIds()
    {
        var npcs = GameObject.FindObjectsOfType<NavTargetPoint>();
        int count = 0;

        foreach (var npc in npcs)
        {
            var so = new SerializedObject(npc);
            var prop = so.FindProperty("pointId");
            if (string.IsNullOrEmpty(prop.stringValue))
            {
                string newId = $"{npc.gameObject.scene.name}_{npc.gameObject.name}_{Guid.NewGuid().ToString().Substring(0, 8)}";
                prop.stringValue = newId;
                so.ApplyModifiedProperties();
                EditorUtility.SetDirty(npc);
                count++;
            }
        }

        Debug.Log($"[NPC ID Generator] Сгенерировано ID для {count} NPC");
    }
}
#endif
