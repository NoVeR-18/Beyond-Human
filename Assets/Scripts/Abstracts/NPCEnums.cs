
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
        //Eat,
        //FightClub,
        //Fishing,
        //Travel,
        Hide,
        //Camp,
        Hunt,
        Chill,
        Patrol

    }

    public enum DialogueContext
    {
        Idle,
        Work,
        Trade,
        Guard,
        Sleep,
        Hunt,
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
        CombatJoin = 1 << 6,
        //add more reasons as needed
    }
    public enum FactionType
    {
        None,
        Player,
        Guards,
        Bandits,
        Mages,
        Undead,
        Animals
        // Add more factions as needed
    }
}