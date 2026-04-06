using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
public class SleepSystem : MonoBehaviour
{
    [Header("UI Компоненты")]
    [SerializeField] private GameObject sleepPanel;
    [SerializeField] private TextMeshProUGUI sleepHoursText;
    [SerializeField] private Button sleepButton;
    [SerializeField] private Button cancelButton;
    [SerializeField] private Button addHourButton;
    [SerializeField] private Button removeHourButton;
    [SerializeField] private float textSpeed = 0.05f;
    public TextMeshProUGUI Text;
    public GameObject image;
    [Header("Настройки сна")]
    [SerializeField] private int minSleepHours = 1;
    [SerializeField] private int maxSleepHours = 12;
    private Coroutine currentTyping;
    private NPCManager nPCManager;
    private GameTimeManager GameTimeManager;
    private SanitySystem sanitySystem;
    private Pills pills;
    private int selectedHours = 8;
    private bool isSleeping = false;
    private float LastSleepHour = GameTimeManager.initialTimeHours;

    void Start()
    {
        image.SetActive(false);
        nPCManager = FindObjectOfType<NPCManager>();
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

    public void Visualize(string text)
    {
        image.SetActive(true);

        if (currentTyping != null)
            StopCoroutine(currentTyping);

        currentTyping = StartCoroutine(TypeText(text));
    }

    IEnumerator TypeText(string text)
    {
        Text.text = "";

        foreach (char c in text)
        {
            Text.text += c;
            yield return new WaitForSeconds(textSpeed);
        }


        yield return new WaitForSeconds(3f);


        image.SetActive(false);
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
        nPCManager.CurrentQuestionNum = 0;
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
            Visualize($"I dont want to sleep now");
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