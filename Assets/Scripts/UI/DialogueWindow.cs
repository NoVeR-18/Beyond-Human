using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class DialogueWindow : MonoBehaviour, IPointerDownHandler
{
    [Header("UI Elements")]
    [SerializeField] private GameObject panel;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI dialogueText;

    [Header("Typing Settings")]
    [SerializeField] private float typingSpeed = 0.02f;

    private Coroutine typingCoroutine;
    private bool isTyping;
    private string fullText;

    private void Awake()
    {
        HideDialogue();
    }

    /// <summary>Показать реплику с именем и текстом</summary>
    public void ShowDialogue(string speaker, string text)
    {
        panel.SetActive(true);
        nameText.text = speaker;
        fullText = text;

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeText(fullText));
    }

    private IEnumerator TypeText(string text)
    {
        isTyping = true;
        dialogueText.text = "";

        foreach (char c in text)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }

    /// <summary>Пропустить печать и сразу показать полный текст</summary>
    public void SkipTyping()
    {
        if (!isTyping) return;

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        dialogueText.text = fullText;
        isTyping = false;
    }

    public void HideDialogue() => panel.SetActive(false);
    public void ShowDialogue() => panel.SetActive(true);

    public void OnPointerDown(PointerEventData eventData)
    {
        NextLine();
    }

    public bool IsVisible => panel.activeSelf;
    public bool IsTyping => isTyping;

    private int currentLineIndex;

    string speaker;
    private DialogueData currentDialogue;

    public void StartDialogue(DialogueData dialogue, string speaker = "")
    {
        if (dialogue == null || dialogue.lines == null || dialogue.lines.Count == 0)
        {
            Debug.LogWarning("DialogueManager: пустой диалог или нет реплик.");
            return;
        }

        currentDialogue = dialogue;
        this.speaker = speaker;
        currentLineIndex = 0;
        ShowDialogue();
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
        string speakerName = line.speaker == SpeakerType.Player
            ? "you"
            : speaker;

        ShowDialogue(speakerName, line.Text);
    }

    public void NextLine()
    {
        if (IsTyping)
        {
            SkipTyping();
        }
        else
        {
            currentLineIndex++;
            ShowLine();
        }
    }

    private void EndDialogue()
    {
        HideDialogue();

        DialogueManager.Instance.CloseDialogue(currentDialogue?.result);

        currentDialogue = null;
        currentLineIndex = 0;
    }

}
