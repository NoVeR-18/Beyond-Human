using NPCEnums;
using UnityEngine;

namespace Assets.Scripts.NPC.States
{

    public class TradeState : INPCState, IInteractableState
    {
        private NPCController npc;


        public TradeState(NPCController npc)
        {
            this.npc = npc;
        }

        public void Enter()
        {
            npc.Agent.ResetPath();
            npc.StartContextDialogue(DialogueContext.Trade);
        }

        public void Update()
        {
            if (npc.CanSeePlayer(out var player) && npc.isAggressive)
            {
                npc.target = player;
                npc.StateMachine.ChangeState(new ChaseState(npc));
                return;
            }

        }

        public void Exit()
        {
            npc.StopContextDialogue();
        }


        public void Interact(NPCController npc)
        {
            Debug.Log("Open trade window.");
            UIManager.Instance.tradeWindow.Show();
        }
    }
}