using Quests;
using UnityEngine;

/// <summary>
/// Триггер для выполнения шагов квеста (например, "посетить место").
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class QuestLocationTrigger : MonoBehaviour
{
    [Tooltip("ID квестов, которые будут отмечены при входе в эту зону")]
    [SerializeField] private string zoneID;

    private void Reset()
    {
        // Чтобы коллайдер сразу стал триггером
        var col = GetComponent<Collider2D>();
        if (col != null)
            col.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            QuestManager.Instance?.ReportExplore(zoneID);

        }
    }
}
