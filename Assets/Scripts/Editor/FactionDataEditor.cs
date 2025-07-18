using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(FactionData))]
public class FactionDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        FactionData faction = (FactionData)target;

        GUILayout.Space(10);

        if (GUILayout.Button("➕ Fill in relations with other factions"))
        {
            AutoFillRelations(faction);
        }

        if (GUILayout.Button("🔁 Add or Update yourself to all other factions"))
        {
            AddSelfToOtherFactions(faction);
        }
    }

    private void AutoFillRelations(FactionData self)
    {
        var allFactions = LoadAllFactions();
        int updated = 0;

        foreach (var other in allFactions)
        {
            if (other == self) continue;

            // Найдём отношение из other к self
            var otherRelToSelf = other.relations.Find(r => r.otherFaction == self);
            int attitude = otherRelToSelf != null ? otherRelToSelf.attitude : 0;

            var existingRel = self.relations.Find(r => r.otherFaction == other);
            if (existingRel != null)
            {
                existingRel.attitude = attitude;
            }
            else
            {
                self.relations.Add(new FactionData.FactionRelation
                {
                    otherFaction = other,
                    attitude = attitude
                });
            }

            updated++;
        }

        EditorUtility.SetDirty(self);
        Debug.Log($"[{self.factionType}] — relations get from {updated} factions.");
    }

    private void AddSelfToOtherFactions(FactionData self)
    {
        var allFactions = LoadAllFactions();
        foreach (var other in allFactions)
        {
            if (other == self) continue;

            // Найдём отношение из self к other
            var sourceRel = self.relations.Find(r => r.otherFaction == other);
            int attitude = sourceRel != null ? sourceRel.attitude : 0;

            // Найдём отношение в other к self
            var targetRel = other.relations.Find(r => r.otherFaction == self);
            if (targetRel != null)
            {
                targetRel.attitude = attitude;
            }
            else
            {
                other.relations.Add(new FactionData.FactionRelation
                {
                    otherFaction = self,
                    attitude = attitude
                });
            }

            EditorUtility.SetDirty(other);
        }


        Debug.Log($"[{self.factionType}] add as object to other factions");
    }

    private List<FactionData> LoadAllFactions()
    {
        var assets = AssetDatabase.FindAssets("t:FactionData");
        var result = new List<FactionData>();

        foreach (var guid in assets)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var faction = AssetDatabase.LoadAssetAtPath<FactionData>(path);
            if (faction != null)
                result.Add(faction);
        }

        return result;
    }
}
