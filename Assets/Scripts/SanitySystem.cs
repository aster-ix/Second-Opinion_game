using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class SanitySystem : MonoBehaviour
{
    [Header("Настройки рассудка")]
    [SerializeField] private float maxSanity = 100f;
    [SerializeField] private float minSanity = 0f;
    [SerializeField] private float startingSanity = 100f;

    [Header("Влияние сна на рассудок")]
    [SerializeField] private float optimalSleepHours = 8f;
    [SerializeField] private float maxPenaltySanity = 15f;

    [Header("UI Компоненты")]
    [SerializeField] private TextMeshProUGUI sanityText;
    [SerializeField] private Image sanitySlider;

    private class SleepRecord
    {
        public float sleepTime; 
        public float duration; 
    }

    private float currentSanity;
    private GameTimeManager gameTimeManager;

    private List<SleepRecord> sleepRecords = new List<SleepRecord>();
    private float lastPenaltyCheckTime = 0f;
    private bool isSleeping = false;

    void Start()
    {
        gameTimeManager = FindObjectOfType<GameTimeManager>();
        currentSanity = startingSanity;
        lastPenaltyCheckTime = gameTimeManager.CurrentTimeHours;

        UpdateSanityDisplay();
    }

    void Update()
    {
        if (gameTimeManager != null && !isSleeping)
        {
            CheckAndApplySanityPenalty();
        }
    }

    float GetSleepHoursLast24Hours()
    {
        float currentGameTime = gameTimeManager.CurrentTimeHours;
        
        float sleepTotal = 0f;

        for (int i = sleepRecords.Count - 1; i >= 0; i--)
        {
            if (currentGameTime - sleepRecords[i].sleepTime > 24f)
            {
                sleepRecords.RemoveAt(i);
            }
            else
            {
                sleepTotal += sleepRecords[i].duration;
            }
        }

        return sleepTotal;
    }

    void CheckAndApplySanityPenalty()
    {
        float currentTime = gameTimeManager.CurrentTimeHours;

        if (currentTime - lastPenaltyCheckTime >= 24f)
        {
            float sleepHoursLast24 = GetSleepHoursLast24Hours();
            float sleepRatio = sleepHoursLast24 / optimalSleepHours;

            if (sleepRatio < 1f)
            {
                float penaltyPercent = 1f - sleepRatio;
                float penalty = penaltyPercent * maxPenaltySanity;

                if (penalty > 0.01f)
                {
                    ChangeSanity(-penalty);
                    Debug.Log($"За последние 24 часа: {sleepHoursLast24:F1} / {optimalSleepHours} ч сна. Штраф: -{penalty:F2} рассудка");
                }
            }
            else
            {
                Debug.Log($"За последние 24 часа: {sleepHoursLast24:F1} / {optimalSleepHours} ч сна. Штрафа нет");
            }

            lastPenaltyCheckTime = currentTime;
        }
    }

    public void ApplySleep(float sleepHours)
    {
        if (isSleeping) return;

        isSleeping = true;
        AddSleepHours(sleepHours);
        isSleeping = false;
    }

    public void AddSleepHours(float hours)
    {
        float currentTime = gameTimeManager.CurrentTimeHours;

        SleepRecord record = new SleepRecord
        {
            sleepTime = currentTime,
            duration = hours
        };

        sleepRecords.Add(record);
        Debug.Log($"Добавлен сон {hours} часов в {currentTime:F2}");
        if (record.duration >= optimalSleepHours)
        {
            lastPenaltyCheckTime = currentTime;
        }
    }

    public void ChangeSanity(float amount)
    {
        currentSanity = Mathf.Clamp(currentSanity + amount, minSanity, maxSanity);
        UpdateSanityDisplay();
    }

    void UpdateSanityDisplay()
    {
        if (sanityText != null)
        {
            sanityText.text = $"{Mathf.RoundToInt(currentSanity)}/{Mathf.RoundToInt(maxSanity)}";
        }

        if (sanitySlider != null)
        {
            sanitySlider.material.SetFloat("_Value", currentSanity / maxSanity);
        }
    }

    public float GetCurrentSanity()
    {
        return currentSanity;
    }

    public void SetSanity(float value)
    {
        currentSanity = Mathf.Clamp(value, minSanity, maxSanity);
        UpdateSanityDisplay();
    }

    public float GetSleepHoursLast24()
    {
        return GetSleepHoursLast24Hours();
    }
}