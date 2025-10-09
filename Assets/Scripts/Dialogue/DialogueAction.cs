using Assets.Scripts.NPC;
using UnityEngine;

[System.Serializable]
public abstract class DialogueAction : ScriptableObject
{
    public abstract void Execute(NPCController npc, PlayerController player);
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
