using Assets.Scripts.NPC.Dialogue;
using NPCEnums;
using UnityEngine;

namespace Assets.Scripts.NPC.States
{

    public class SleepState : INPCState, IInterruptible
    {
        private NPCController npc;

        public SleepState(NPCController npc)
        {
            this.npc = npc;
        }

        public void Enter()
        {
            npc.Agent.ResetPath();
            npc.Animator.SetBool("IsSleeping", true);

            npc.StartContextDialogue(DialogueContext.Sleep);
        }

        public void Update()
        {
            // NPC спит, ничего не делает. Может проснуться по времени или если его потревожили
            if (npc.CanSeePlayer(out var player) && npc.isAggressive)
            {
                npc.target = player;
                npc.StateMachine.ChangeState(new ChaseState(npc));
            }
        }

        public void Exit()
        {
            npc.Animator.SetBool("IsSleeping", false);
        }


        public void Interrupt(NPCController source, InterruptReason reason)
        {
            if (reason == InterruptReason.PlayerRunning || reason == InterruptReason.PlayerWalking)
            {
                npc.StateMachine.ChangeState(new IdleState(npc));
                UIFloatingText.Create(npc.transform.position + Vector3.up, "Who are you?");
            }
        }
    }

}