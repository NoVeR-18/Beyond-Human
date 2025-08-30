using Quests;
using TMPro;
using UnityEngine;

public class QuestItemUI : MonoBehaviour
{
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private TMP_Text progressText;

    public void Setup(Quest quest)
    {
        titleText.text = quest.data.title;
        descriptionText.text = quest.data.description;
        progressText.text = quest.isCompleted
            ? "Complete!"
            : $"{quest.currentAmount}/{quest.data.requiredAmount}";
    }
}
