using System;
using TMPro;
using UnityEngine;

public class ClockUI : MonoBehaviour
{
    public TextMeshProUGUI clockText;
    public TextMeshProUGUI dayText;

    private void Start()
    {
        UpdateClock(TimeManager.Instance.CurrentTime);
        TimeManager.Instance.OnTimeChanged += UpdateClock;
        TimeManager.Instance.OnDayChanged += Instance_OnDayChanged;

        Instance_OnDayChanged(TimeManager.Instance.CurrentTime.Day);
        UpdateClock(TimeManager.Instance.CurrentTime);

    }

    private void Instance_OnDayChanged(DayOfWeek obj)
    {
        dayText.text = obj.ToString();
    }

    private void UpdateClock(GameTime time)
    {
        clockText.text = time.ToString(); // формат вида 08:00
    }

    private void OnDestroy()
    {
        if (TimeManager.Instance != null)
            TimeManager.Instance.OnTimeChanged -= UpdateClock;
    }
}
