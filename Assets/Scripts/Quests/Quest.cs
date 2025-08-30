using NPCEnums;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Quest
{
    public QuestData data;
    public int currentAmount;
    public bool isCompleted;

    public Quest(QuestData data)
    {
        this.data = data;
        currentAmount = 0;
        isCompleted = false;
    }

    public void AddProgress()
    {
        if (isCompleted) return;

        currentAmount++;
        if (currentAmount >= data.requiredAmount)
            Complete();

    }

    private void Complete()
    {
        isCompleted = true;
        Debug.Log($"Квест {data.title} завершен!");

        // ✅ награда: опыт + золото
        // PlayerStats.Instance.AddExp(data.rewardExp);
        // PlayerStats.Instance.AddGold(data.rewardGold);

        // ✅ награда: предметы
        if (data.itemRewards != null)
        {
            foreach (var reward in data.itemRewards)
            {
                Inventory.Instance.Add(reward.item, reward.amount);
                Debug.Log($"Получен предмет: {reward.item.name} x{reward.amount}");
            }
        }

        // ✅ награда: фракции
        if (data.factionRewards != null)
        {
            foreach (var reward in data.factionRewards)
            {
                FactionManager.Instance.ModifyReputation(FactionType.Player, reward.factionId, reward.reputationChange);

                Debug.Log($"Изменена репутация с {reward.factionId}: {reward.reputationChange}");
            }
        }
    }
}

[System.Serializable]
public class QuestSaveData
{
    public string questId;
    public int currentAmount;
    public bool isCompleted;
}
[System.Serializable]
public class QuestSaveWrapper
{
    public List<QuestSaveData> activeQuests = new();
    public List<QuestSaveData> completedQuests = new();
}
