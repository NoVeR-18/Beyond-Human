using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ClockUI : MonoBehaviour
{
    public TextMeshProUGUI clockText;

    private void Start()
    {
        UpdateClock(TimeManager.Instance.CurrentTime);
        TimeManager.Instance.OnMinuteChanged += UpdateClock;
    }

    private void UpdateClock(GameTime time)
    {
        clockText.text = time.ToString(); // формат вида 08:00
    }

    private void OnDestroy()
    {
        if (TimeManager.Instance != null)
            TimeManager.Instance.OnMinuteChanged -= UpdateClock;
    }
}
