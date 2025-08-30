using GameUtils.Utils;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class QuestSaveManager
{
    public static void Save()
    {
        SaveUtils.EnsureDirectory();

        QuestSaveWrapper wrapper = new QuestSaveWrapper();

        foreach (var q in QuestManager.Instance.activeQuests)
        {
            wrapper.activeQuests.Add(new QuestSaveData
            {
                questId = q.data.questId,
                currentAmount = q.currentAmount,
                isCompleted = q.isCompleted
            });
        }

        foreach (var q in QuestManager.Instance.completedQuests)
        {
            wrapper.completedQuests.Add(new QuestSaveData
            {
                questId = q.data.questId,
                currentAmount = q.currentAmount,
                isCompleted = q.isCompleted
            });
        }

        string json = JsonUtility.ToJson(wrapper, true);
        File.WriteAllText(SaveUtils.QuestsFile, json);

        Debug.Log("Quests saves");
    }

    public static void Load(List<QuestData> allQuests)
    {
        if (!File.Exists(SaveUtils.QuestsFile))
        {
            Debug.Log("File is missing loading quests skiping");
            return;
        }

        string json = File.ReadAllText(SaveUtils.QuestsFile);
        QuestSaveWrapper wrapper = JsonUtility.FromJson<QuestSaveWrapper>(json);

        QuestManager.Instance.activeQuests.Clear();
        QuestManager.Instance.completedQuests.Clear();

        foreach (var sd in wrapper.activeQuests)
        {
            QuestData qd = allQuests.Find(q => q.questId == sd.questId);
            if (qd != null)
            {
                Quest quest = new Quest(qd)
                {
                    currentAmount = sd.currentAmount,
                    isCompleted = sd.isCompleted
                };
                QuestManager.Instance.activeQuests.Add(quest);
            }
        }

        foreach (var sd in wrapper.completedQuests)
        {
            QuestData qd = allQuests.Find(q => q.questId == sd.questId);
            if (qd != null)
            {
                Quest quest = new Quest(qd)
                {
                    currentAmount = sd.currentAmount,
                    isCompleted = sd.isCompleted
                };
                QuestManager.Instance.completedQuests.Add(quest);
            }
        }

        Debug.Log("Quests loading");
    }
}
