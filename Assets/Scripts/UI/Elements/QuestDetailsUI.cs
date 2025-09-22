using Quests;
using System.Text;
using TMPro;
using UnityEngine;

public class QuestDetailsUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private TMP_Text objectiveText;   // новое: что нужно сделать
    [SerializeField] private TMP_Text rewardsText;

    public void Show(Quest quest)
    {
        if (quest == null || quest.data == null)
        {
            Clear();
            return;
        }

        titleText.text = quest.data.title;
        descriptionText.text = quest.data.description;

        objectiveText.text = BuildObjectiveText(quest);

        rewardsText.text = BuildRewardsText(quest.data);
    }

    private string BuildObjectiveText(Quest quest)
    {
        var data = quest.data;
        var sb = new StringBuilder();

        if (quest.isCompleted)
        {
            sb.Append("<b>Task:</b> Complete");
            return sb.ToString();
        }

        // Варианты формулировок по типу квеста
        switch (data.questType)
        {
            case QuestType.Kill:
                // targetId — имя существа/объекта; если у тебя есть targetFactionId, можно туда ориентироваться
                sb.Append("<b>Task:</b> Kill");
                sb.Append(!string.IsNullOrEmpty(data.targetId) ? data.targetId : "Target");
                sb.AppendFormat($" ({quest.currentAmount}/{data.requiredAmount})");
                break;

            case QuestType.Collect:
                sb.Append("<b>Task:</b> Collect");
                sb.Append(!string.IsNullOrEmpty(data.targetId) ? data.targetId : "items");
                sb.AppendFormat($" ({quest.currentAmount}/{data.requiredAmount})");
                break;

            case QuestType.Talk:
                sb.Append("<b>Task:</b> Talk with");
                sb.Append(!string.IsNullOrEmpty(data.targetId) ? data.targetId : "NPC");
                break;

            case QuestType.Explore:
                sb.Append("<b>Task:</b> Visit the location");
                sb.Append(!string.IsNullOrEmpty(data.targetId) ? data.targetId : data.locationId.ToString());
                break;

            default:
                sb.Append("<b>Task:</b> ");
                sb.Append(data.description);
                break;
        }

        return sb.ToString();
    }

    private string BuildRewardsText(QuestData data)
    {
        StringBuilder sb = new StringBuilder();

        // Фракционные награды
        if (data.factionRewards != null && data.factionRewards.Count > 0)
        {
            sb.AppendLine("<b>Award (reputation):</b>");
            foreach (var reward in data.factionRewards)
            {
                sb.AppendLine($"- {reward.factionId}: {reward.reputationChange:+#;-#;0}");
            }
            sb.AppendLine();
        }

        // Награды-предметы
        if (data.itemRewards != null && data.itemRewards.Count > 0)
        {
            sb.AppendLine("<b>Award (items):</b>");
            foreach (var reward in data.itemRewards)
            {
                if (reward.item != null)
                    sb.AppendLine($"- {reward.item.itemName} x{reward.amount}");
            }
        }


        return sb.ToString();
    }

    private void Clear()
    {
        titleText.text = "";
        descriptionText.text = "";
        objectiveText.text = "";
        rewardsText.text = "";
    }
}
