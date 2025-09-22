using Quests;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class QuestWindow : UIWindow
{
    [Header("UI References")]
    [SerializeField] private GameObject content;
    [SerializeField] private Transform locationListParent;   // родитель для локаций
    [SerializeField] private QuestLocationGroupUI locationGroupPrefab; // префаб группы локации
    [SerializeField] private QuestDetailsUI questDetailsUI;  // правая панель деталей

    private readonly List<GameObject> activeGroups = new();

    public override void Show()
    {
        content.SetActive(true);
        if (QuestManager.Instance != null)
            QuestManager.OnQuestUpdated += RefreshUI;
        RefreshUI();
    }

    public override void Hide()
    {
        content.SetActive(false);
        if (QuestManager.Instance != null)
            QuestManager.OnQuestUpdated -= RefreshUI;
        ClearUI();
    }

    private void RefreshUI()
    {
        ClearUI();
        if (QuestManager.Instance == null) return;

        // Группируем квесты по локациям
        var grouped = QuestManager.Instance.activeQuests
            .GroupBy(q => q.data.locationId);

        foreach (var group in grouped)
        {
            if (!group.Any()) continue;

            QuestLocationGroupUI groupUI = Instantiate(locationGroupPrefab, locationListParent);
            activeGroups.Add(groupUI.gameObject);

            if (groupUI != null)
            {
                groupUI.Setup(group.Key, group.ToList(), ShowQuestDetails);
            }
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(
    locationListParent.transform.parent as RectTransform
);
    }

    private void ClearUI()
    {
        foreach (var go in activeGroups)
            Destroy(go);
        activeGroups.Clear();
    }

    private void ShowQuestDetails(Quest quest)
    {
        questDetailsUI.Show(quest);

        LayoutRebuilder.ForceRebuildLayoutImmediate(content.transform.parent as RectTransform);
    }
}
