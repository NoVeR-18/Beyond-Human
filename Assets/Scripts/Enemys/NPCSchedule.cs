using UnityEngine;
using System.Collections.Generic;

public class NPCSchedule : MonoBehaviour
{
    [System.Serializable]
    public class ScheduleEntry
    {
        public int hour;
        public int minute;
        public NPCAction action;
    }

    public enum NPCAction
    {
        Idle,
        GoToWork,
        GoHome,
        Sleep,
        Wander
    }

    public List<ScheduleEntry> dailySchedule;

   // private NPCController npc;

    private void Start()
    {
       // npc = GetComponent<NPCController>();
        TimeManager.Instance.OnMinuteChanged += OnTimeChanged;
    }

    private void OnTimeChanged(GameTime time)
    {
        foreach (var entry in dailySchedule)
        {
            if (entry.hour == time.Hour && entry.minute == time.Minute)
            {
         //       npc.PerformAction(entry.action);
                break;
            }
        }
    }

    private void OnDestroy()
    {
        if (TimeManager.Instance != null)
            TimeManager.Instance.OnMinuteChanged -= OnTimeChanged;
    }
}
