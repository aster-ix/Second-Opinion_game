using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;

public class Pills : MonoBehaviour
{
    [Header("Настройки таблетки")]
    [SerializeField] private float sanityRestoreAmount = 20f;
    [SerializeField] private int PillsAmount = 3;
    [SerializeField] private float textSpeed = 0.05f;

    public TextMeshProUGUI Text;
    public GameObject image;
    public Animator animator;

    public bool isUsed = false;

    private SanitySystem sanitySystem;

    private Coroutine currentTyping;

    void Start()
    {
        sanitySystem = FindObjectOfType<SanitySystem>();
        image.SetActive(false); 
    }

    public void UsePill()
    {
        if (PillsAmount > 0)
        {
            if (!isUsed)
            {
                animator.Play("Piils_Popils", 0, 0.0f);
                PillsAmount -= 1;
                isUsed = true;
                Visualize($"{PillsAmount} pills remain");
            }
            else
            {
                Visualize("I've already taken a pill");
            }
        }
        else
        {
            Visualize("There's no pills left");
        }
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