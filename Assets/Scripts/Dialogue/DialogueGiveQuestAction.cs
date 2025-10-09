using Assets.Scripts.NPC;
using Quests;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/Actions/Give Quest")]
public class DialogueGiveQuestAction : DialogueAction
{
    public QuestData quest;

    public override void Execute(NPCController npc, PlayerController player)
    {
        if (quest == null) { Debug.LogWarning("GiveQuestAction: quest is null"); return; }
        QuestManager.Instance.AddQuest(quest);
    }
}