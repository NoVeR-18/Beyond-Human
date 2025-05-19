using System.Xml;
using UnityEngine;

public class IdleState : INPCState
{
    private NPCController npc;
    private float idleTimer = 2f;

    public void Enter(NPCController npc)
    {
        this.npc = npc;
        npc.agent.ResetPath();
        idleTimer = Random.Range(1.5f, 3f);
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

        idleTimer -= Time.deltaTime;
        if (idleTimer <= 0)
        {
            npc.stateMachine.ChangeState(new RoamState());
        }
    }
}
