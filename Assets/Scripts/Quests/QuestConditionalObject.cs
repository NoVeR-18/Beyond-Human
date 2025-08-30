using UnityEngine;
namespace Quests
{

    public enum QuestConditionType { QuestActive, QuestCompleted, QuestNotStarted }

    public class QuestConditionalObject : MonoBehaviour
    {
        [Header("Quest Condition")]
        public string questId;
        public QuestConditionType conditionType;

        [Header("Target Object (what will be enabled/disabled)")]
        public GameObject targetObject;
        public bool invert;

        private void Start()
        {
            UpdateState();
            QuestManager.OnQuestUpdated += UpdateState;
        }

        private void OnDestroy()
        {
            QuestManager.OnQuestUpdated -= UpdateState;
        }

        private void UpdateState()
        {
            if (QuestManager.Instance == null) return;

            bool shouldEnable = false;

            switch (conditionType)
            {
                case QuestConditionType.QuestActive:
                    shouldEnable = QuestManager.Instance.IsQuestActive(questId);
                    break;

                case QuestConditionType.QuestCompleted:
                    shouldEnable = QuestManager.Instance.IsQuestCompleted(questId);
                    break;

                case QuestConditionType.QuestNotStarted:
                    shouldEnable = !QuestManager.Instance.IsQuestActive(questId)
                                   && !QuestManager.Instance.IsQuestCompleted(questId);
                    break;
            }

            if (invert) shouldEnable = !shouldEnable;

            if (targetObject != null)
                targetObject.SetActive(shouldEnable);
            else
                gameObject.SetActive(shouldEnable);
        }
    }

}