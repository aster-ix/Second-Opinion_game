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
    [SerializeField] public GameObject HUD;
    [SerializeField] public GameObject NoteBook;
    [SerializeField] public GameObject Block;
    [Header("Настройки")]
    [SerializeField] private float textSpeed = 0.05f;
    [SerializeField] private string jsonFileName = "Monologue";
    [SerializeField] private float delayBetweenLines = 2f;
    private List<string> lines = new List<string>();
    private Shiza shiza;
    private int currentLineIndex = 0;
    private bool isPlaying = false;
    private bool isTyping = false;
    private Coroutine typingCoroutine;
    public CameraRotation cameraRotation;
    void Start()
    {
        shiza = FindObjectOfType<Shiza>();
        LoadLines();
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);
        
    }



    void LoadLines()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>(jsonFileName);

        if (jsonFile != null)
        {
            Wrapper wrapper = JsonUtility.FromJson<Wrapper>(jsonFile.text);

            int randomIndex = Random.Range(0, wrapper.monologues.Count);

            lines = wrapper.monologues[randomIndex].lines;
        }
    }

    [System.Serializable]
    private class Monologue
    {
        public List<string> lines;
    }

    [System.Serializable]
    private class Wrapper
    {
        public List<Monologue> monologues;
    }

    public void StartMonologue()
    {
        if (isPlaying) return;
        LoadLines();
        currentLineIndex = 0;
        isPlaying = true;

        Block.SetActive(true);
        HUD.SetActive(false);
        NoteBook.SetActive(false);

        StartCoroutine(StartMonologueWithDelay());
    }

    IEnumerator StartMonologueWithDelay()
    {
 
        if (dialoguePanel != null)
            dialoguePanel.SetActive(true);


        ShowCurrentLine();

        yield return new WaitForSeconds(2f);

        if (shiza.AppearedA == true)
        {
            cameraRotation.RotateLeft();
        }
        else
        {
            cameraRotation.RotateRight();
        }

        
    }
    IEnumerator Waiter(float seconds)
    {
        yield return new WaitForSeconds(seconds);
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

        foreach (char c in text)
        {
            textField.text += c;
            yield return new WaitForSeconds(textSpeed);
        }

        isTyping = false;

        yield return new WaitForSeconds(delayBetweenLines);

        currentLineIndex++;
        ShowCurrentLine();
    }


    void EndMonologue()
    {
        isPlaying = false;
        isTyping = false;
        if (shiza.AppearedA == true)
        {
            cameraRotation.RotateRight();
        }
        else
        {
            cameraRotation.RotateLeft();
        }
        Block.SetActive(false);
        HUD.SetActive(true);
        NoteBook.SetActive(true);
        StartCoroutine(HidePanel());

    }

    IEnumerator HidePanel()
    {
        yield return new WaitForSeconds(1.5f);


        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        textField.text = "";

        if (shiza != null)
        {
            shiza.DestroyCurrentShiza();
        }
    }

}