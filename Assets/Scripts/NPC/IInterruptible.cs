using NPCEnums;

namespace Assets.Scripts.NPC
{
    public interface IInterruptible
    {
        void Interrupt(NPCController npc, InterruptReason reason);

        private bool CanJoin()
        {
            return true;
        }
    }

    public interface IInteractableState
    {
        void Interact(NPCController npc);
    }
}
