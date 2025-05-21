using System;
using UnityEngine;

[Serializable]
public struct GameTime : IEquatable<GameTime>
{
    public int Hour;
    public int Minute;
    public DayOfWeek Day;

    public GameTime(int hour, int minute, DayOfWeek days)
    {
        Hour = hour;
        Day = days;
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
    public GameTime startTime = new GameTime(8, 0, DayOfWeek.Monday);

    public GameTime CurrentTime;

    public event Action<GameTime> OnTimeChanged;
    public event Action<DayOfWeek> OnDayChanged;

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

        OnTimeChanged?.Invoke(CurrentTime);
    }

    private void AdvanceDay()
    {
        CurrentTime.Day++;
        CurrentTime.Day = (DayOfWeek)(((int)CurrentTime.Day + 1) % 7);
        OnDayChanged?.Invoke(CurrentTime.Day);
    }

    public void SaveTime()
    {
        PlayerPrefs.SetInt("Hour", CurrentTime.Hour);
        PlayerPrefs.SetInt("Minute", CurrentTime.Minute);
        PlayerPrefs.SetInt("WeekDay", (int)CurrentTime.Day);
    }

    public void LoadTime()
    {
        int hour = PlayerPrefs.GetInt("Hour", startTime.Hour);
        int minute = PlayerPrefs.GetInt("Minute", startTime.Minute);
        DayOfWeek CurrentWeekDay = (DayOfWeek)PlayerPrefs.GetInt("WeekDay", 1);

        CurrentTime = new GameTime(hour, minute, CurrentWeekDay);
    }

    private void OnApplicationQuit()
    {
        SaveTime();
    }
}
