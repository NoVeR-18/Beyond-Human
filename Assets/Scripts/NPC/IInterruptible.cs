using NPCEnums;

namespace Assets.Scripts.NPC
{
    public interface IInterruptible
    {
        void Interrupt(NPCController npc, InterruptReason reason);
    }

    public interface IInteractableState
    {
        void Interact(NPCController npc);
    }
}
