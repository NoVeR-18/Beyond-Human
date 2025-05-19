using System.Xml;
using UnityEngine;

public class IdleState : INPCState
{
    private NPCController npc;
    private float idleTimer = 2f;
    
    public IdleState(NPCController npc)
    {
        this.npc = npc;
    }

    public void Enter()
    {
        npc.Agent.ResetPath();
        idleTimer = Random.Range(1.5f, 3f);
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

        idleTimer -= Time.deltaTime;
        if (idleTimer <= 0)
        {
            npc.StateMachine.ChangeState(new RoamState(npc));
        }
    }
}
