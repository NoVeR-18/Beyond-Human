using System;

namespace Assets.Scripts.NPC
{

    [Serializable]
    public class NPCStateMachine
    {
        public INPCState CurrentState { get; private set; }
        private NPCController npc;

        public NPCStateMachine(NPCController npc)
        {
            this.npc = npc;
        }

        public void ChangeState(INPCState newState)
        {
            CurrentState?.Exit();
            CurrentState = newState;
            CurrentState.Enter();
            npc.StateName = newState.ToString();
        }
    }

}