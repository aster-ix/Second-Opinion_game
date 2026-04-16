using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShizaMonologue : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI textField;
    [SerializeField] private GameObject dialoguePanel;

    [Header("Настройки")]
    [SerializeField] private float textSpeed = 0.05f;
    [SerializeField] private string jsonFileName = "Monologue";
    [SerializeField] private Button clickZone;
    private List<string> lines = new List<string>();
    private Shiza shiza;
    private int currentLineIndex = 0;
    private bool isPlaying = false;
    private bool isTyping = false;
    private Coroutine typingCoroutine;

    void Start()
    {
        shiza = FindObjectOfType<Shiza>();
        LoadLines();
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);
        SetupClickHandler();
    }
    void SetupClickHandler()
    {
        if (clickZone != null)
        {
            clickZone.onClick.AddListener(OnClickHandler);
        }
    }
    void OnClickHandler()
    {
        if (!isPlaying) return;

        if (isTyping)
        {
            StopTypingAndShowFullText();
        }
        else
        {
            NextLine();
        }
    }

    void LoadLines()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>(jsonFileName);

        if (jsonFile != null)
        {
            Wrapper wrapper = JsonUtility.FromJson<Wrapper>(jsonFile.text);
            lines = wrapper.lines;
        }
        else
        {
            return;
        }
    }

    [System.Serializable]
    private class Wrapper
    {
        public List<string> lines;
    }

    public void StartMonologue()
    {
        if (isPlaying) return;

        currentLineIndex = 0;
        isPlaying = true;

        if (dialoguePanel != null)
            dialoguePanel.SetActive(true);

        ShowCurrentLine();
    }

    void ShowCurrentLine()
    {
        if (currentLineIndex < lines.Count)
        {
            if (typingCoroutine != null)
                StopCoroutine(typingCoroutine);

            typingCoroutine = StartCoroutine(TypeText(lines[currentLineIndex]));
        }
        else
        {
            EndMonologue();
        }
    }

    IEnumerator TypeText(string text)
    {
        isTyping = true;
        textField.text = "";

        foreach (char c in text.ToCharArray())
        {
            textField.text += c;
            yield return new WaitForSeconds(textSpeed);
        }

        isTyping = false;
    }

    void StopTypingAndShowFullText()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        textField.text = lines[currentLineIndex];
        isTyping = false;
    }

    void NextLine()
    {
        currentLineIndex++;
        ShowCurrentLine();
    }

    void EndMonologue()
    {
        isPlaying = false;
        isTyping = false;

        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        textField.text = "";
        if (shiza != null)
        {
            shiza.DestroyCurrentShiza(); 
        }
    }

}