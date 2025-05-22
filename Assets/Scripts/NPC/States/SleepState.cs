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

        public void Interrupt(NPCController npc, InterruptReason reason)
        {
            if (reason == InterruptReason.PlayerNearby || reason == InterruptReason.AlarmTriggered)
            {
                Debug.Log("NPC просыпается по причине: " + reason);
                npc.StateMachine.ChangeState(new IdleState(npc));
            }
        }
    }

}