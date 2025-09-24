using Assets.Scripts.NPC;
using Quests;
using UnityEngine;

public abstract class DialogueAction : ScriptableObject
{
    public abstract void Execute(NPCController npc, PlayerController player);
}

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

[CreateAssetMenu(menuName = "Dialogue/Actions/Hire Companion")]
public class DialogueHireCompanionAction : DialogueAction
{
    public override void Execute(NPCController npc, PlayerController player)
    {
        if (PartyManager.Instance.AddMember(new Character(npc.GetComponent<NPCController>())))
            Destroy(npc);
    }
}

//[CreateAssetMenu(menuName = "Dialogue/Actions/Unlock Barrier")]

public class DialogueUnlockBarrierAction : DialogueAction
{
    public string barrierId;

    public override void Execute(NPCController npc, PlayerController player)
    {
        //WQuestBarrierManager.Instance.Unlock(barrierId);
    }
}
