using Assets.Scripts.NPC;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    [Header("UI Window")]
    [SerializeField] private DialogueWindow dialogueWindow;

    public static DialogueManager Instance;
    private NPCController currentNpc;
    private PlayerController currentPlayer;

    public void StartDialogue(DialogueData dialogue, NPCController npc, PlayerController player)
    {
        dialogueWindow.StartDialogue(dialogue);
        currentNpc = npc;
        currentPlayer = player;
    }


    private void Awake()
    {
        if (Instance == null) Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        if (dialogueWindow == null)
            Debug.LogError("DialogueManager: не назначено DialogueWindow!");
    }
    public void CloseDialogue(DialogueAction action)
    {
        if (action != null)
            try
            {
                action?.Execute(currentNpc, currentPlayer);
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"DialogueManager: ошибка выполнения DialogueAction — {ex}");
            }
        currentNpc = null;
        currentPlayer = null;
    }
    /// <summary>Запускает диалог. Передай NPC и игрока</summary>

}
