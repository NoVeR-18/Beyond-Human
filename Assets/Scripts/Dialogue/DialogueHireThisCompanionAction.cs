using Assets.Scripts.NPC;
using UnityEngine;


[CreateAssetMenu(menuName = "Dialogue/Actions/Hire This Companion")]
public class DialogueHireThisCompanionAction : DialogueAction
{
    public override void Execute(NPCController npc, PlayerController player)
    {
        if (PartyManager.Instance.AddMember(new Character(npc.GetComponent<NPCController>())))
            npc.Destroy();
    }
}