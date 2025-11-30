using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TimerUI : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI dayText;
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI endTimeText;
    public Slider progressSlider; 

    private int _day;

    private int _startHour;
    private int _startMinute;

    private int _hour;
    private int _min;

    private int _endHour;
    private int _endMin;

    //DEBUG USE
    private void Start()
    {
        //Debug Data
    }
    private void UpdateAllUI()
    {
        dayText.text = $"Day {_day}";
        timeText.text = $"{_hour:D2}:{_min:D2}";
        endTimeText.text = $"{_endHour:D2}:{_endMin:D2}";

        int startTimeInMinutes = _startHour * 60 + _startMinute;
        int currentTimeInMinutes = _hour * 60 + _min;
        int endTimeInMinutes = _endHour * 60 + _endMin;

        if (endTimeInMinutes > 0)
        {
            progressSlider.value = (float)(currentTimeInMinutes - startTimeInMinutes) / (endTimeInMinutes - startTimeInMinutes);
        }
        else
        {
            progressSlider.value = 0f;
        }
    }

    public void SetDay(int newDay)
    {
        _day = newDay;
        dayText.text = $"Day {_day}";
    }

    public void SetStartTime(int newHour, int newMin)
    {
        _startHour = newHour;
        _startMinute = newMin;

        UpdateAllUI();
    }

    public void SetTime(int newHour, int newMin)
    {
        _hour = newHour;
        _min = newMin;

        UpdateAllUI();
    }

    public void SetEndTime(int newEndHour, int newEndMin)
    {
        _endHour = newEndHour;
        _endMin = newEndMin;

        UpdateAllUI();
    }
}