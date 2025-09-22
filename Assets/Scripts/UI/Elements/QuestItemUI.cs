using Quests;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestItemUI : MonoBehaviour
{
    [SerializeField] private TMP_Text questTitle;
    [SerializeField] private Button button;

    private Quest quest;

    public void Setup(Quest quest, System.Action onClick)
    {
        this.quest = quest;
        questTitle.text = quest.data.title;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => onClick?.Invoke());
    }
}
