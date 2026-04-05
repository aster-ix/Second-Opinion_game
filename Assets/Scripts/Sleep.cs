using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SleepSystem : MonoBehaviour
{
    [Header("UI Компоненты")]
    [SerializeField] private GameObject sleepPanel;
    [SerializeField] private TextMeshProUGUI sleepHoursText;
    [SerializeField] private Button sleepButton;
    [SerializeField] private Button cancelButton;
    [SerializeField] private Button addHourButton;
    [SerializeField] private Button removeHourButton;

    [Header("Настройки сна")]
    [SerializeField] private int minSleepHours = 1;
    [SerializeField] private int maxSleepHours = 12;

    private GameTimeManager GameTimeManager;
    private SanitySystem sanitySystem;
    private Pills pills;
    private int selectedHours = 8;
    private bool isSleeping = false;
    private float LastSleepHour = GameTimeManager.initialTimeHours;

    void Start()
    {
        GameTimeManager = FindObjectOfType<GameTimeManager>();
        pills = FindObjectOfType<Pills>();
        sanitySystem = FindObjectOfType<SanitySystem>();
        if (addHourButton != null)
            addHourButton.onClick.AddListener(AddHour);

        if (removeHourButton != null)
            removeHourButton.onClick.AddListener(RemoveHour);

        if (sleepButton != null)
            sleepButton.onClick.AddListener(StartSleep);

        if (cancelButton != null)
            cancelButton.onClick.AddListener(CloseSleepPanel);
        if (sleepPanel != null)
            sleepPanel.SetActive(false);

        UpdateSleepTimeDisplay();
    }
    

    public void OpenSleepPanel()
    {
        
        if (isSleeping) return;
        if (sleepPanel != null)
            sleepPanel.SetActive(true);

        if (GameTimeManager != null)
            GameTimeManager.ToggleTime();
        if (pills.isUsed == true)
        {
            addHourButton.enabled = false;
            removeHourButton.enabled = false;
            sleepButton.enabled = true;
            selectedHours = 12;
            UpdateSleepTimeDisplay();
        }
        else
        {
            CheckBlockSleep();
            selectedHours = 8;
            addHourButton.enabled = true;
            removeHourButton.enabled = true;
            UpdateSleepTimeDisplay();
        }
        

        
    }

    public void CloseSleepPanel()
    {

        if (sleepPanel != null)
            sleepPanel.SetActive(false);
        if (GameTimeManager != null)
            GameTimeManager.ToggleTime();

    }

    public void AddHour()
    {
        selectedHours++;
        if (selectedHours > maxSleepHours)
            selectedHours = maxSleepHours;

        UpdateSleepTimeDisplay();
    }

    public void RemoveHour()
    {
        selectedHours--;
        if (selectedHours < minSleepHours)
            selectedHours = minSleepHours;

        UpdateSleepTimeDisplay();
    }

    void UpdateSleepTimeDisplay()
    {
        if (sleepHoursText != null)
        {
            sleepHoursText.text = $"{selectedHours} h";
        }
    }

    public void StartSleep()
    {
        if (isSleeping) return;
        float sleepTimeMinutes = selectedHours * 60f;
        GameTimeManager.AddTime(selectedHours, 0);


        if (sanitySystem != null)
        {
            sanitySystem.ApplySleep(selectedHours);
        }
        if(pills.isUsed == true)
        {
            pills.pillEffect();
            addHourButton.enabled = true;
            removeHourButton.enabled = true;
        }
        isSleeping = true;

        
        
        EndSleep();
    }

    void EndSleep()
    {
        if (!isSleeping) return;

        isSleeping = false;
        CloseSleepPanel();
        var (hours, minutes) = GameTimeManager.GetCurrentTime();
        LastSleepHour = hours + (minutes / 60f);
        

        
    }
    public void CheckBlockSleep()
    {
        float currentTime = GameTimeManager.CurrentTimeHours;
        float blockEndTime = LastSleepHour + 3f;

        if (currentTime < blockEndTime)
        {
            sleepButton.enabled = false;

            float remaining = blockEndTime - currentTime;
            Debug.Log($"Вы не можете спать еще {remaining:F1} часов");
        }
        else
        {
            sleepButton.enabled = true;
        }
    }
    public bool IsSleeping()
    {
        return isSleeping;
    }
}