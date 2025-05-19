using UnityEngine;

public class ChaseState : INPCState
{
    private NPCController npc;

    public void Enter(NPCController npc)
    {
        this.npc = npc;
    }

    public void Exit()
    {
        npc.agent.ResetPath();
    }

    public void Update()
    {
        if (npc.target == null)
        {
            npc.stateMachine.ChangeState(new IdleState());
            return;
        }

        npc.agent.SetDestination(npc.target.position);

        float distance = Vector3.Distance(npc.transform.position, npc.target.position);
        if (distance <= 1.5f) // Радиус поимки
        {
            Debug.Log("Поймал игрока!");
            npc.agent.ResetPath();
            // Здесь можно вставить анимацию, переход в другое состояние и т.п.
        }

        if (!npc.CanSeePlayer(out var seenPlayer))
        {
            npc.stateMachine.ChangeState(new IdleState()); // Можно сделать GoToLastSeenPositionState
        }
    }
}
