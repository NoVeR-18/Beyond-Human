using UnityEngine;

public class WorkState : INPCState
{
    private NPCController npc;
    private float workDuration;
    private float timer;

    public WorkState(NPCController npc)
    {
        this.npc = npc;
    }

    public void Enter()
    {
        workDuration = Random.Range(5f, 15f); // можно заменить на более реалистичное значение из расписания
        timer = workDuration;

        npc.Animator.SetBool("IsWorking", true); // должен быть триггер/бул в Animator
    }

    public void Update()
    {
        if (npc.CanSeePlayer(out var player) && npc.isAggressive)
        {
            npc.target = player;
            npc.StateMachine.ChangeState(new ChaseState(npc));
            return;
        }

        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            npc.StateMachine.ChangeState(new IdleState(npc)); // после работы можно отправить домой или в следующее состояние
        }
    }

    public void Exit()
    {
        npc.Animator.SetBool("IsWorking", false);
    }
}
