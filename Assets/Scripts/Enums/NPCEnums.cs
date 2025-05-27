
namespace NPCEnums
{
    public enum NPCActivityType
    {
        Sleep,
        Work,
        Idle,
        Wander,
        Trade,
        Guard,
        Eat,
        FightClub,
        Fishing,
        Travel,
        Hide,
        Camp,
        Hunt,
        Chill
    }

    public enum DialogueContext
    {
        Idle,
        Work,
        Trade,
        Threatened,
        Sleep,
        Greeting,
        Quest,
        Scared
    }
    [System.Flags]
    public enum InterruptReason
    {
        None = 0,
        PlayerWalking = 1 << 0,
        PlayerRunning = 1 << 1,
        Lockpicking = 1 << 2,
        ScreamHelp = 1 << 3,
        ChaseAlert = 1 << 4,
        Hunting = 1 << 5,
        // Добавляй по мере нужды
    }
}