public class SleepState : INPCState
{
    private NPCController npc;

    public SleepState(NPCController npc)
    {
        this.npc = npc;
    }

    public void Enter()
    {
        npc.Agent.ResetPath();
        npc.Animator.SetBool("IsSleeping", true);
    }

    public void Update()
    {
        // NPC ����, ������ �� ������. ����� ���������� �� ������� ��� ���� ��� �����������
        if (npc.CanSeePlayer(out var player) && npc.isAggressive)
        {
            npc.target = player;
            npc.StateMachine.ChangeState(new ChaseState(npc));
        }
    }

    public void Exit()
    {
        npc.Animator.SetBool("IsSleeping", false);
    }
}
