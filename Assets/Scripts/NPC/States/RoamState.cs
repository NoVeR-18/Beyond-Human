using GameUtils.Utils;
using UnityEngine;

public class RoamState : INPCState
{
    private NPCController npc;
    private Vector3 roamPos;

    public void Enter(NPCController npc)
    {
        this.npc = npc;
        roamPos = npc.transform.position + Utils.GetRandomDir() * Random.Range(3f, 7f);
        npc.agent.SetDestination(roamPos);
    }

    public void Exit() { }

    public void Update()
    {
        if (npc.CanSeePlayer(out var player) && npc.isAggressive)
        {
            npc.target = player;
            npc.stateMachine.ChangeState(new ChaseState());
            return;
        }

        if (!npc.agent.pathPending && npc.agent.remainingDistance < npc.agent.stoppingDistance)
        {
            npc.stateMachine.ChangeState(new IdleState());
        }
    }
}
