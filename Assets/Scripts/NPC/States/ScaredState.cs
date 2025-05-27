using NPCEnums;
using UnityEngine;

namespace Assets.Scripts.NPC.States
{
    public class ScaredState : INPCState
    {
        private NPCController npc;
        private Transform threat;
        private float fleeDistance = 10f;

        public ScaredState(NPCController npc, Transform threat)
        {
            this.npc = npc;
            this.threat = threat;
        }

        public void Enter()
        {
            Vector2 fleeDirection = (npc.transform.position - threat.position).normalized;
            Vector2 fleeTarget = npc.transform.position + (Vector3)(fleeDirection * fleeDistance);
            npc.Agent.SetDestination(fleeTarget);

            npc.emitter.Activate(InterruptReason.ScreamHelp, 4f, 1f);
        }

        public void Update()
        {
            // Можно добавить проверку "достаточно ли далеко"
        }

        public void Exit() { }
    }
}
