using NPCEnums;
using System.Collections.Generic;
using UnityEngine;

namespace Quests
{

    public enum QuestType { Kill, Collect, Talk, Explore }

    public enum QuestTargetType { ById, ByFaction }

    [CreateAssetMenu(fileName = "New Quest", menuName = "Quests/Quest")]
    public class QuestData : ScriptableObject
    {
        [Header("Main")]
        public string questId;
        public string title;
        [TextArea] public string description;
        public LocationId locationId; // where complete the quest

        [Header("Task")]
        public QuestType questType;

        [Tooltip("Для Kill/Collect")]
        public QuestTargetType targetType;

        [Tooltip("ID target (example, Wolf001). Use, if TargetType = ById")]
        public string targetId;

        [Tooltip("Faction target (example, Animals). Use, if TargetType = ByFaction")]
        public FactionType targetFactionId;

        [Tooltip("Need count (kill/collect etc)")]
        public int requiredAmount = 1;

        [Tooltip("Change faction after complete")]
        public List<FactionReward> factionRewards;

        [Tooltip("Items, as reward")]
        public List<ItemReward> itemRewards;
    }

    [System.Serializable]
    public class FactionReward
    {
        public FactionType factionId;
        public int reputationChange;
    }

    [System.Serializable]
    public class ItemReward
    {
        public Item item;
        public int amount = 1;
    }

}