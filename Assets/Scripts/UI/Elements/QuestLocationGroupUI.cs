using Quests;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestLocationGroupUI : MonoBehaviour
{
    [SerializeField] private TMPro.TMP_Text locationNameText;
    [SerializeField] private Button headerButton;
    [SerializeField] private Transform hidenVield;
    [SerializeField] private Transform questListParent;
    [SerializeField] private QuestItemUI questItemPrefab;

    private bool expanded = false;

    public void Setup(LocationId locationId, List<Quest> quests, Action<Quest> onQuestSelected)
    {
        locationNameText.text = locationId.ToString();

        // Очистка детей
        foreach (Transform child in questListParent)
            Destroy(child.gameObject);

        foreach (var quest in quests)
        {
            QuestItemUI questUI = Instantiate(questItemPrefab, questListParent);
            questUI.Setup(quest, () => onQuestSelected?.Invoke(quest));
        }

        hidenVield.gameObject.SetActive(expanded);
        headerButton.onClick.AddListener(ToggleExpand);

        LayoutRebuilder.ForceRebuildLayoutImmediate(
    hidenVield.transform.parent as RectTransform
);
    }

    public void ToggleExpand()
    {
        expanded = !expanded;
        hidenVield.gameObject.SetActive(expanded);
        LayoutRebuilder.ForceRebuildLayoutImmediate(
    hidenVield.transform.parent as RectTransform
);
    }
}
