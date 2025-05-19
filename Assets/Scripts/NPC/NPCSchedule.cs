using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "NPC/Schedule")]
public class NPCSchedule : ScriptableObject
{
    public List<ScheduleEntry> entries;
}

[System.Serializable]
public class ScheduleEntry
{
    public int hour;
    public int minute;
    public string actionName; // например: "GoHome", "GoToWork"
    public Transform destination;
}