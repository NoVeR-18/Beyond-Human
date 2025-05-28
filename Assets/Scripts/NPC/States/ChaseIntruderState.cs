using Assets.Scripts.NPC;
using Assets.Scripts.NPC.States;
using NPCEnums;
using UnityEngine;

public class ChaseIntruderState : INPCState
{
    private readonly NPCController npc;
    private Transform target;

    public ChaseIntruderState(NPCController npc)
    {
        this.npc = npc;
        this.target = GameObject.FindGameObjectWithTag("Player").transform;
    }

    public void Enter()
    {
        npc.target = target;
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
        npc.emitter.Activate(InterruptReason.ChaseAlert);

        float distance = Vector3.Distance(npc.transform.position, npc.target.position);
        if (distance <= 1.5f)
        {
            Debug.Log("Поймал игрока!");
            npc.Agent.ResetPath();
        }

        if (!npc.CanSeePlayer(out var seenPlayer))
        {
            npc.StateMachine.ChangeState(new IdleState(npc));
        }
    }
}
