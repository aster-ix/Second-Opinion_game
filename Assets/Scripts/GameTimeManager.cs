using UnityEngine;
using TMPro;

public class GameTimeManager : MonoBehaviour
{
    [Header("Настройки времени")]
    [SerializeField] private float initialTimeHours = 0f;
    [SerializeField] private float initialTimeMinutes = 0f;
    [SerializeField] private bool autoStart = true;

    [Header("Текст сюда")]
    [SerializeField] private TextMeshProUGUI timeText;

    private float currentTimeMinutes;
    private bool isRunning = true;


    public float CurrentTimeMinutes
    {
        get => currentTimeMinutes;
        set
        {
            currentTimeMinutes = Mathf.Max(0, value);
            UpdateTimeDisplay();
        }
    }


    public float CurrentTimeHours
    {
        get => currentTimeMinutes / 60f;
        set
        {
            currentTimeMinutes = Mathf.Max(0, value * 60f);
            UpdateTimeDisplay();
        }
    }

    void Start()
    {

        currentTimeMinutes = initialTimeMinutes + (initialTimeHours * 60f);

        if (autoStart)
        {
            if (!isRunning)
            {
                ToggleTime();
            }
        }

        UpdateTimeDisplay();
    }

    void Update()
    {
        if (isRunning)
        {

            currentTimeMinutes += Time.deltaTime;
            UpdateTimeDisplay();
        }
    }

    void UpdateTimeDisplay()
    {
        if (timeText == null) return;


        int totalMinutes = Mathf.FloorToInt(currentTimeMinutes);
        int hours = (totalMinutes / 60) % 24; 
        int minutes = totalMinutes % 60;


        timeText.text = string.Format("{0:D2}:{1:D2}", hours, minutes);
    }

    // ВРЕМЯ МОЖЕТ БЫТЬ -
    public void AddTime(float hours, float minutes)
    {
        currentTimeMinutes += (hours * 60f) + minutes;
        currentTimeMinutes = Mathf.Max(0, currentTimeMinutes);
        UpdateTimeDisplay();
    }

    public void SetTime(int hours, int minutes)
    {
        currentTimeMinutes = Mathf.Max(0, (hours * 60f) + minutes);
        UpdateTimeDisplay();
    }


    public (int hours, int minutes) GetCurrentTime()
    {
        int totalMinutes = Mathf.FloorToInt(currentTimeMinutes);
        int hours = (totalMinutes / 60) % 24; 
        int minutes = totalMinutes % 60;
        return (hours, minutes);
    }

    public void ToggleTime()
    {
        isRunning = !isRunning;
    }

    public void ResetTime()
    {
        currentTimeMinutes = 0;
        UpdateTimeDisplay();
    }
}