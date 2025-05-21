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
        workDuration = Random.Range(5f, 15f); // ����� �������� �� ����� ������������ �������� �� ����������
        timer = workDuration;

        npc.Animator.SetBool("IsWorking", true); // ������ ���� �������/��� � Animator
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
            npc.StateMachine.ChangeState(new IdleState(npc)); // ����� ������ ����� ��������� ����� ��� � ��������� ���������
        }
    }

    public void Exit()
    {
        npc.Animator.SetBool("IsWorking", false);
    }
}
