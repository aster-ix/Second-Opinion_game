using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogUIManagerScript : MonoBehaviour
{
    public TMP_Text   choiceText;       // текст реплики NPC
    public Transform  choiceParent;  // панель с кнопками вариантов
    public GameObject buttonPrefab;
    public GameObject arrowOpen;     // иконка стрелки когда варианты скрыты  
    public GameObject arrowClose;    // иконка стрелки когда варианты открыты 

    [CanBeNull] public DialogManager dialogManager;
    public NPCManager NPCManager;
    public Button NextDialogButton;

    private List<GameObject> _buttons = new();
    private bool _choicesVisible = true;
    public bool isOver = false;
    private  bool isTyping = false;
    
    [SerializeField]
    private float _textSpeed = 0.5f;

    private  Coroutine typingCoroutine;
    void Start()
    {
        ShowCurrentDialog();
    }


    public void ToggleChoices()
    {
        _choicesVisible = !_choicesVisible;

        foreach (var btn in _buttons)
            btn.SetActive(_choicesVisible);

        if (arrowOpen  != null) arrowOpen.SetActive(!_choicesVisible);
        if (arrowClose != null) arrowClose.SetActive(_choicesVisible);
    }

    public void PreviousDialog()
    {
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        dialogManager?.PreviousDialog();
        ShowCurrentDialog();
    }


    public void ShowCurrentDialog()
    {
        if (dialogManager?.currentDialog == null) return;
        isOver = false;
        //choiceText.text = dialogManager.currentDialog.text;
        typingCoroutine = StartCoroutine(TypeText(dialogManager.currentDialog.text));
        BuildChoiceButtons();
    }

    public void BuildChoiceButtons()
    {
    foreach (var btn in _buttons)
        Destroy(btn);
    _buttons.Clear();

    if (dialogManager.choices == null) return;

    float yPos = 238f;

    foreach (Choice choice in dialogManager.choices)
    {
        yPos -= 128f;

        GameObject btn = Instantiate(buttonPrefab, choiceParent);
        btn.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, yPos);
        btn.SetActive(_choicesVisible); 

        Choice captured = choice;
        if (!captured.isLast)
        {
            btn.GetComponent<Button>().onClick.AddListener(() => OnChoiceClicked(captured));
        }
        else
        {
            btn.GetComponent<Button>().onClick.AddListener(CloseDialog);
        }

        btn.GetComponentInChildren<TMP_Text>().text = choice.text;
        btn.GetComponent<Button>().interactable = choice.nextNode != null && choice.nextNode.isActive;

        _buttons.Add(btn);
    }
}

    void OnChoiceClicked(Choice choice)
    {
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        dialogManager.NextDialog(dialogManager.choices.IndexOf(choice));
        ShowCurrentDialog();
    }

    public void CloseDialog()
    {
        //NextDialogButton.interactable = true;
        dialogManager = null;
        choiceText.text = "";
        NPCManager.DeleteNPC();
        foreach (var btn in _buttons) Destroy(btn);
        _buttons.Clear();
        isOver = true;
    }
    IEnumerator TypeText(string text)
    {
        isTyping = true;
        choiceText.text = "";

        foreach (char c in text.ToCharArray())
        {
            choiceText.text += c;
            yield return new WaitForSeconds(_textSpeed);
        }

        isTyping = false;
    }
}