using TMPro;
using UnityEngine;

public class DialogueWindow : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_Text dialogueText;

    public void ShowDialogue(string text)
    {
        dialogueText.text = text;
        panel.SetActive(true);
    }

    public void HideDialogue()
    {
        panel.SetActive(false);
    }

    public bool IsVisible => panel.activeSelf;
}
