
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
        Camp
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
    }
    public enum InterruptReason
    {
        PlayerNearby,
        PlayerInteracted,
        AlarmTriggered,
        TimeToWakeUp,
        DangerDetected,
        // Add more reasons as needed
    }
}