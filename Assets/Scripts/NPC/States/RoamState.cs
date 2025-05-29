using Assets.Scripts.NPC;
using Assets.Scripts.NPC.States;
using GameUtils.Utils;
using NPCEnums;
using UnityEngine;
using UnityEngine.AI;

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
        if (npc.CurrentHouse != null && npc.CurrentFloor != npc.CurrentHouse.entranceFloor)
        {
            roamPos = HouseRoamUtils.GetRandomNavMeshPointInsideFloor(npc.CurrentHouse.floors[npc.CurrentFloor]);
        }
        else
        {
            // Улица
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

        npc.StartContextDialogue(DialogueContext.Idle);
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
            npc.StateMachine.ChangeState(new RoamState(npc));
        }
    }
}
