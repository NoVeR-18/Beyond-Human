using System;
using UnityEngine;

[Serializable]
public struct GameTime : IEquatable<GameTime>
{
    public int Hour;
    public int Minute;

    public GameTime(int hour, int minute)
    {
        Hour = hour;
        Minute = minute;
    }

    public bool Equals(GameTime other)
    {
        return Hour == other.Hour && Minute == other.Minute;
    }

    public override string ToString()
    {
        return $"{Hour:D2}:{Minute:D2}";
    }
}


public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance;

    [Header("Time Settings")]
    public float timeScale = 60f; // 1 реальная секунда = 1 игровая минута
    public GameTime startTime = new GameTime(8, 0);

    public GameTime CurrentTime;
    public int CurrentDay { get; private set; } = 1;
    public DayOfWeek CurrentWeekDay { get; private set; } = DayOfWeek.Monday;

    public event Action<GameTime> OnMinuteChanged;
    public event Action<int, DayOfWeek> OnDayChanged;

    private float timeAccumulator;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        LoadTime();
    }

    private void Update()
    {
        timeAccumulator += Time.deltaTime * timeScale;

        while (timeAccumulator >= 60f)
        {
            timeAccumulator -= 60f;
            AdvanceMinute();
        }
    }

    private void AdvanceMinute()
    {
        CurrentTime.Minute++;
        if (CurrentTime.Minute >= 60)
        {
            CurrentTime.Minute = 0;
            CurrentTime.Hour++;
            if (CurrentTime.Hour >= 24)
            {
                CurrentTime.Hour = 0;
                AdvanceDay();
            }
        }

        OnMinuteChanged?.Invoke(CurrentTime);
    }

    private void AdvanceDay()
    {
        CurrentDay++;
        CurrentWeekDay = (DayOfWeek)(((int)CurrentWeekDay + 1) % 7);
        OnDayChanged?.Invoke(CurrentDay, CurrentWeekDay);
    }

    public void SaveTime()
    {
        PlayerPrefs.SetInt("Hour", CurrentTime.Hour);
        PlayerPrefs.SetInt("Minute", CurrentTime.Minute);
        PlayerPrefs.SetInt("Day", CurrentDay);
        PlayerPrefs.SetInt("WeekDay", (int)CurrentWeekDay);
    }

    public void LoadTime()
    {
        int hour = PlayerPrefs.GetInt("Hour", startTime.Hour);
        int minute = PlayerPrefs.GetInt("Minute", startTime.Minute);
        CurrentTime = new GameTime(hour, minute);
        CurrentDay = PlayerPrefs.GetInt("Day", 1);
        CurrentWeekDay = (DayOfWeek)PlayerPrefs.GetInt("WeekDay", 1);
    }

    private void OnApplicationQuit()
    {
        SaveTime();
    }
}
