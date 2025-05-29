using NPCEnums;
using UnityEngine;

namespace Assets.Scripts.NPC.States
{
    public class HuntState : INPCState
    {
        private NPCController npc;
        private Vector3 center;
        private float wanderRadius = 4f;
        private float idleTimer;

        public HuntState(NPCController npc)
        {
            this.npc = npc;
            center = npc.transform.position; // Базовая точка охоты
        }

        public void Enter()
        {
            MoveToRandomPoint();

            npc.StartContextDialogue(DialogueContext.Hunt);
        }

        public void Exit() { }

        public void Update()
        {
            if (npc.CanSeePlayer(out var player))
            {
                npc.target = player;
                npc.StateMachine.ChangeState(new ChaseState(npc, NPCEnums.InterruptReason.Hunting));
                return;
            }

            if (!npc.Agent.pathPending && npc.Agent.remainingDistance < 0.2f)
            {
                idleTimer -= Time.deltaTime;
                if (idleTimer <= 0)
                {
                    MoveToRandomPoint();
                }
            }
        }

        private void MoveToRandomPoint()
        {
            Vector2 randomOffset = Random.insideUnitCircle * wanderRadius;
            Vector3 target = center + new Vector3(randomOffset.x, randomOffset.y);
            npc.Agent.SetDestination(target);
            idleTimer = Random.Range(1f, 3f);
        }
    }
}
