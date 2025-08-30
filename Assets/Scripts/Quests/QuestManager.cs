using Assets.Scripts.NPC;
using System.Collections.Generic;
using UnityEngine;

namespace Quests
{

    public class QuestManager : MonoBehaviour
    {
        public static QuestManager Instance;

        public List<Quest> activeQuests = new();
        public List<Quest> completedQuests = new();

        public delegate void QuestUpdated();
        public static event QuestUpdated OnQuestUpdated;
        [SerializeField] private QuestDatabase questDatabase;


        private void Awake()
        {
            if (Instance == null) Instance = this;
            QuestSaveManager.Load(questDatabase.quests);
        }

        public void AddQuest(QuestData questData)
        {

            if (activeQuests.Exists(q => q.data.questId == questData.questId))
                return;

            Quest quest = new Quest(questData);
            activeQuests.Add(quest);

            Debug.Log($"Get Quest: {questData.title}");
            OnQuestUpdated?.Invoke();
        }

        private void CheckCompletion()
        {
            for (int i = activeQuests.Count - 1; i >= 0; i--)
            {
                if (activeQuests[i].isCompleted)
                {
                    completedQuests.Add(activeQuests[i]);
                    activeQuests.RemoveAt(i);
                }
            }
        }
        private void OnApplicationQuit()
        {
            QuestSaveManager.Save();
        }

        public void OnEnemyKilled(NPCController enemy)
        {
            foreach (var quest in activeQuests)
            {
                if (quest.data.questType != QuestType.Kill) continue;

                bool validTarget = false;

                if (quest.data.targetType == QuestTargetType.ById && enemy.name == quest.data.targetId)
                    validTarget = true;

                if (quest.data.targetType == QuestTargetType.ByFaction && enemy.FactionType == quest.data.targetFactionId)
                    validTarget = true;

                if (validTarget)
                {
                    quest.AddProgress();
                }
            }
        }

        public void ReportCollect(string itemId)
        {
            foreach (var quest in activeQuests)
                if (quest.data.questType == QuestType.Collect)
                    if (quest.data.targetId == itemId)
                        quest.AddProgress();

            CheckCompletion();
            OnQuestUpdated?.Invoke();
        }

        public void ReportTalk(string npcId)
        {
            foreach (var quest in activeQuests)
                if (quest.data.questType == QuestType.Talk)
                    if (quest.data.targetId == npcId)
                        quest.AddProgress();

            CheckCompletion();
            OnQuestUpdated?.Invoke();
        }

        public void ReportExplore(string zoneId)
        {
            foreach (var quest in activeQuests)
                if (quest.data.questType == QuestType.Explore)
                    if (quest.data.targetId == zoneId)
                        quest.AddProgress();

            CheckCompletion();
            OnQuestUpdated?.Invoke();
        }

        public bool IsQuestActive(string questId)
        {
            return activeQuests.Exists(q => q.data.questId == questId);
        }

        public bool IsQuestCompleted(string questId)
        {
            return completedQuests.Exists(q => q.data.questId == questId);
        }

        public void ReportProgress(QuestType type, string targetId)
        {
            switch (type)
            {
                case QuestType.Kill:
                    OnEnemyKilled(new NPCController { name = targetId });
                    break;
                case QuestType.Collect:
                    ReportCollect(targetId);
                    break;
                case QuestType.Talk:
                    ReportTalk(targetId);
                    break;
                case QuestType.Explore:
                    ReportExplore(targetId);
                    break;
            }
        }

    }
}