using UnityEngine;

public class TradeState : INPCState
{
    private NPCController npc;
    private float tradeDuration;
    private float timer;

    public TradeState(NPCController npc)
    {
        this.npc = npc;
    }

    public void Enter()
    {
        tradeDuration = Random.Range(10f, 20f); // ��� ������������� ����� � ����������� �� ����������
        timer = tradeDuration;

        npc.Animator.SetTrigger("StartTrading");
        // �������� �������� �������������� � ������� � �������� ��������, ���� ����� �����
    }

    public void Update()
    {
        if (npc.CanSeePlayer(out var player) && npc.isAggressive)
        {
            npc.target = player;
            npc.StateMachine.ChangeState(new ChaseState(npc));
            return;
        }

    }

    public void Exit()
    {
        npc.Animator.SetTrigger("StopTrading");
    }
}
