using UnityEngine;

public class Pills : MonoBehaviour
{
    [Header("Настройки таблетки")]
    [SerializeField] private float sanityRestoreAmount = 20f;
    [SerializeField] private int PillsAmount = 3;
    public bool isUsed = false;

    private SanitySystem sanitySystem;
    
    void Start()
    {
        sanitySystem = FindObjectOfType<SanitySystem>();
        
    }

    
    public void UsePill()
    {
        if (PillsAmount > 0)
        {
            if (!isUsed)
            {
                PillsAmount -= 1;
                isUsed = true;
                Debug.Log($"Таблеток осталось {PillsAmount}");
            }
            else
            {
                Debug.Log($"Вы уже использовали таблетку");
            }
            
        }
        else
        {
            Debug.Log($"Таблеток не осталось");
        }
        
    }
    public void AddPills(int amount)
    {
        PillsAmount += amount;
    }
    public void pillEffect()
    {
        
        sanitySystem.ChangeSanity(sanityRestoreAmount);
        isUsed = false;
        
    }
}