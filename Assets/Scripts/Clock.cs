using UnityEngine;

public class Clock : MonoBehaviour
{
    private GameTimeManager gameTimeManager;
    public GameObject Minutes;
    public GameObject Hours;
    float currentMinutes = 0.0f;

    void Start()
    {
        gameTimeManager = FindObjectOfType<GameTimeManager>();
        if (gameTimeManager != null)
            currentMinutes = gameTimeManager.CurrentTimeMinutes;
    }

    void Update()
    {
        if (gameTimeManager != null)
        {
            currentMinutes = gameTimeManager.CurrentTimeMinutes;
            UpdateClock();
        }
    }

    public void UpdateClock()
    {
        
        float minutesAngle = currentMinutes * 6f;

        
        float hoursAngle = (currentMinutes / 60f) * 30f;

        
        Minutes.transform.localEulerAngles = new Vector3(-minutesAngle, 0f, 0f);
        Hours.transform.localEulerAngles = new Vector3(-hoursAngle, 0f, 0f);
    }
}