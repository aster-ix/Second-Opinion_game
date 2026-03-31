using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SanitySystem : MonoBehaviour
{
    [Header("Настройки рассудка")]
    [SerializeField] private float maxSanity = 100f; 
    [SerializeField] private float minSanity = 0f; 
    [SerializeField] private float startingSanity = 100f; 
    [SerializeField] private float sanityDecayPerHour = 2f; 

    [Header("Влияние сна на рассудок")]
    [SerializeField] private float optimalSleepHours = 8f; 
    [SerializeField] private float maxBonusSanity = 20f; 
    [SerializeField] private float maxPenaltySanity = 30f; 

    [SerializeField] private TextMeshProUGUI sanityText; 
    [SerializeField] private Slider sanitySlider; 

    private float currentSanity;
    private GameTimeManager GameTimeManager;
    private float lastUpdateTime; 
    private bool isSleeping = false;

    void Start()
    {
        GameTimeManager = FindObjectOfType<GameTimeManager>();
        currentSanity = startingSanity;
        lastUpdateTime = GameTimeManager.CurrentTimeMinutes;

        UpdateSanityDisplay();
    }

    void Update()
    {
        if (GameTimeManager != null && !isSleeping)
        {
            UpdateSanityOverTime();
        }
    }

    
    void UpdateSanityOverTime()
    {
        float currentTime = GameTimeManager.CurrentTimeMinutes;
        float timePassed = currentTime - lastUpdateTime;

        if (timePassed >= 1f) 
        {
            
            float hoursPassed = timePassed / 60f;
            float sanityChange = -sanityDecayPerHour * hoursPassed;

            ChangeSanity(sanityChange);

            lastUpdateTime = currentTime;
        }
    }

    
    public void ApplySleep(int sleepHours)
    {
        isSleeping = true;

        
        float sleepEffect = CalculateSleepEffect(sleepHours);

        ChangeSanity(sleepEffect);

        

        isSleeping = false;
        lastUpdateTime = GameTimeManager.CurrentTimeMinutes; 
    }


    float CalculateSleepEffect(int sleepHours)
    {
        float difference = sleepHours - optimalSleepHours;

        if (Mathf.Abs(difference) < 0.1f) 
        {
            return 0f; 
        }
        else if (difference > 0) 
        {
            
            float bonus = (difference / (24f - optimalSleepHours)) * maxBonusSanity;
            return Mathf.Min(bonus, maxBonusSanity);
        }
        else 
        {
            
            float penalty = (Mathf.Abs(difference) / optimalSleepHours) * maxPenaltySanity;
            return -Mathf.Min(penalty, maxPenaltySanity);
        }
    }
    //РАССУДОК МОЖЕТ БЫТЬ - 
    public void ChangeSanity(float amount)
    {
        float oldSanity = currentSanity;
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
            sanitySlider.value = currentSanity / maxSanity;
        }

    }

    public float GetCurrentSanity()
    {
        return currentSanity;
    }

    public float GetSanityPercent()
    {
        return currentSanity / maxSanity;
    }

    public void SetSanity(float value)
    {
        currentSanity = Mathf.Clamp(value, minSanity, maxSanity);
        UpdateSanityDisplay();
    }

}