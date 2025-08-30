using NPCEnums;
using UnityEngine;

public enum QuestType { Kill, Collect, Talk, Explore }

public enum QuestTargetType { ById, ByFaction }

[CreateAssetMenu(fileName = "New Quest", menuName = "Quests/Quest")]
public class QuestData : ScriptableObject
{
    [Header("Main")]
    public string questId;
    public string title;
    [TextArea] public string description;

    [Header("Task")]
    public QuestType questType;

    [Tooltip("Для Kill/Collect")]
    public QuestTargetType targetType;

    [Tooltip("ID цели (например, Wolf001). Используется, если TargetType = ById")]
    public string targetId;

    [Tooltip("Фракция цели (например, Animals). Используется, если TargetType = ByFaction")]
    public FactionType targetFactionId;

    [Tooltip("Необходимо выполнить (кол-во убитых/собранных и т.д.)")]
    public int requiredAmount = 1;

    [Header("Rewards")]
    public int rewardExp;
    public int rewardGold;

    [Tooltip("Изменение репутации фракции при завершении квеста")]
    public FactionReward[] factionRewards;

    [Tooltip("Предметы, выдаваемые в награду")]
    public ItemReward[] itemRewards;
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
