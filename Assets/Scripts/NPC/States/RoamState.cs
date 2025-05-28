using GameUtils.Utils;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Scripts.NPC.States
{
    public class RoamState : INPCState
    {
        private NPCController npc;
        private Vector3 roamPos;

        public RoamState(NPCController npc)
        {
            this.npc = npc;
        }

        public void Enter()
        {
            if (npc.CurrentHouse != null && npc.CurrentFloor >= 0 && npc.CurrentFloor < npc.CurrentHouse.floors.Count)
            {
                roamPos = HouseRoamUtils.GetRandomNavMeshPointInsideFloor(npc.CurrentHouse.floors[npc.CurrentFloor]);
            }
            else
            {
                // ”лица Ч тоже проверим на NavMesh
                Vector3 rawTarget = npc.transform.position + Utils.GetRandomDir() * Random.Range(3f, 7f);
                if (NavMesh.SamplePosition(rawTarget, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
                {
                    roamPos = hit.position;
                }
                else
                {
                    roamPos = npc.transform.position; // fallback
                }
            }

            npc.Agent.SetDestination(roamPos);
            npc.Animator.SetFloat("Speed", 1f);
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

            if (!npc.Agent.pathPending && npc.Agent.remainingDistance <= npc.Agent.stoppingDistance)
            {
                npc.StateMachine.ChangeState(new IdleState(npc));
            }
        }
    }
}
