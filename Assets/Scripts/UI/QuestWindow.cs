using System.Collections.Generic;
using UnityEngine;

public class QuestWindow : UIWindow
{
    [SerializeField] private GameObject content;
    [SerializeField] private Transform questListParent;   //all parent for quest items
    [SerializeField] private GameObject questItemPrefab;  // prefab for one quest item

    private readonly List<GameObject> activeItems = new();

    public override void Show()
    {
        content.SetActive(true);
        if (QuestManager.Instance != null)
            QuestManager.Instance.OnQuestUpdated += RefreshUI;
        RefreshUI();
    }

    public override void Hide()
    {
        content.SetActive(false);
        if (QuestManager.Instance != null)
            QuestManager.Instance.OnQuestUpdated -= RefreshUI;
        ClearUI();
    }


    private void RefreshUI()
    {
        ClearUI();

        if (QuestManager.Instance == null) return;

        foreach (var quest in QuestManager.Instance.activeQuests)
        {
            GameObject item = Instantiate(questItemPrefab, questListParent);
            activeItems.Add(item);

            QuestItemUI questUI = item.GetComponent<QuestItemUI>();
            if (questUI != null)
                questUI.Setup(quest);
        }
    }

    private void ClearUI()
    {
        foreach (var go in activeItems)
            Destroy(go);

        activeItems.Clear();
    }
}
