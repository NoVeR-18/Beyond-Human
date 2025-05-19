using UnityEngine;
using UnityEngine.AI;

public class GoToLocationState : INPCState
{
    private NPCController npc;
    private Vector3 destination;

    public GoToLocationState(NPCController npc, Vector3 destination)
    {
        this.npc = npc;
        this.destination = destination;
    }

    public void Enter()
    {
        npc.Agent.SetDestination(destination);
        npc.Animator.SetFloat("Speed", 1f);
    }

    public void Update()
    {
        if (!npc.Agent.pathPending && npc.Agent.remainingDistance <= npc.Agent.stoppingDistance)
        {
            npc.StateMachine.ChangeState(new IdleState(npc)); // —тоит на месте, ждет следующее событие
        }
    }

    public void Exit()
    {
        npc.Animator.SetFloat("Speed", 0f);
    }
}
