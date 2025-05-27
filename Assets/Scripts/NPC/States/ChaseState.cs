using NPCEnums;
using UnityEngine;

namespace Assets.Scripts.NPC.States
{

    public class ChaseState : INPCState
    {
        private NPCController npc;
        private InterruptReason alertReason;

        public ChaseState(NPCController npc, InterruptReason reason = InterruptReason.None)
        {
            this.npc = npc;
            alertReason = reason;
        }

        public void Enter()
        {


        }

        public void Exit()
        {
            npc.Agent.ResetPath();
        }

        public void Update()
        {
            if (npc.target == null)
            {
                npc.StateMachine.ChangeState(new IdleState(npc));
                return;
            }

            npc.Agent.SetDestination(npc.target.position);
            npc.emitter.Activate(alertReason);

            float distance = Vector3.Distance(npc.transform.position, npc.target.position);
            if (distance <= 1.5f) // ������ ������
            {
                Debug.Log("������ ������!");
                npc.Agent.ResetPath();
                // ����� ����� �������� ��������, ������� � ������ ��������� � �.�.
            }

            if (!npc.CanSeePlayer(out var seenPlayer))
            {
                npc.StateMachine.ChangeState(new IdleState(npc)); // ����� ������� GoToLastSeenPositionState
            }
        }
    }

}