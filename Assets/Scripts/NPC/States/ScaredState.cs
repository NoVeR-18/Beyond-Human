using GameUtils.Utils;
using UnityEngine;

namespace Assets.Scripts.NPC.States
{
    public class ScaredState : INPCState
    {
        private NPCController npc;
        private Vector3 roamPos;

        public ScaredState(NPCController npc)
        {
            this.npc = npc;
        }

        public void Enter()
        {
            roamPos = npc.transform.position + Utils.GetRandomDir() * Random.Range(3f, 7f);
            npc.Agent.SetDestination(roamPos);
        }

        public void Exit() { }

        public void Update()
        {
            if (npc.CanSeePlayer(out var player) && npc.isAggressive)
            {
                npc.target = player;
                npc.StateMachine.ChangeState(new ChaseState(npc));
                return;
            }

            if (!npc.Agent.pathPending && npc.Agent.remainingDistance < npc.Agent.stoppingDistance)
            {
                npc.StateMachine.ChangeState(new IdleState(npc));
            }
        }
    }
}
