using UnityEngine;

namespace Assets.Scripts.NPC.States
{
    public class QuestGiverState : INPCState, IInteractableState
    {
        private NPCController npc;

        public QuestGiverState(NPCController npc)
        {
            this.npc = npc;
        }

        public void Enter() => npc.Animator.SetBool("IsGivingQuest", true);

        public void Exit() => npc.Animator.SetBool("IsGivingQuest", false);

        public void Update() { }

        public void Interact(NPCController npc)
        {
            Debug.Log("Открыт диалог квеста.");
            //QuestUI.Instance.Open(npc);
        }
    }

}
