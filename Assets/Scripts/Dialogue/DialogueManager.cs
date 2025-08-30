using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI speakerNameText;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private Button nextButton;

    private DialogueData currentDialogue;
    private int currentLineIndex;

    // кто участвует в диалоге
    private GameObject currentNpc;
    private GameObject currentPlayer;

    public bool IsActive => gameObject.activeSelf && currentDialogue != null;

    private void Awake()
    {
        if (nextButton != null)
            nextButton.onClick.AddListener(NextLine);

        gameObject.SetActive(false);
    }

    /// <summary>
    /// Запускает диалог. Передай NPC и игрока, чтобы результат мог их использовать.
    /// </summary>
    public void StartDialogue(DialogueData dialogue, GameObject npc, GameObject player)
    {
        if (dialogue == null || dialogue.lines == null || dialogue.lines.Count == 0)
        {
            Debug.LogWarning("DialogueManager: Пустой диалог или нет реплик.");
            return;
        }

        currentDialogue = dialogue;
        currentNpc = npc;
        currentPlayer = player;

        currentLineIndex = 0;
        gameObject.SetActive(true);
        ShowLine();
    }

    private void ShowLine()
    {
        if (currentDialogue == null || currentLineIndex >= currentDialogue.lines.Count)
        {
            EndDialogue();
            return;
        }

        var line = currentDialogue.lines[currentLineIndex];

        if (speakerNameText) speakerNameText.text = line.speakerName;
        if (dialogueText) dialogueText.text = line.text;
    }

    public void NextLine()
    {
        if (!IsActive) return;

        currentLineIndex++;
        ShowLine();
    }

    private void EndDialogue()
    {
        // спрятать UI заранее, чтобы действие не зависело от состояния канваса
        gameObject.SetActive(false);

        // выполнить итоговое действие, если задано
        try
        {
            currentDialogue?.result?.Execute(currentNpc, currentPlayer);
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"DialogueManager: ошибка выполнения DialogueAction — {ex}");
        }

        // очистить состояние
        currentDialogue = null;
        currentNpc = null;
        currentPlayer = null;
        currentLineIndex = 0;
    }

    /// <summary>
    /// Принудительно оборвать диалог без выполнения результата.
    /// </summary>
    public void CancelDialogue()
    {
        gameObject.SetActive(false);
        currentDialogue = null;
        currentNpc = null;
        currentPlayer = null;
        currentLineIndex = 0;
    }
}
