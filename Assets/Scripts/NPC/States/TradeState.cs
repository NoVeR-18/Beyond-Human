using NPCEnums;

public class TradeState : INPCState
{
    private NPCController npc;


    public TradeState(NPCController npc)
    {
        this.npc = npc;
    }

    public void Enter()
    {
        npc.Speak(DialogueContext.Trade);
        npc.Agent.ResetPath();
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

    }
}
