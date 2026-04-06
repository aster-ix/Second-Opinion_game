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
    public GameObject DialogUI;
    [CanBeNull] public DialogManager dialogManager;
    public NPCManager NPCManager;
    public GameTimeManager gameTimeManager;
    public Shiza shiza;
    public Button NextDialogButton;

    private List<GameObject> _buttons = new();
    private bool _choicesVisible = true;
    public bool isOver = true;

    void Start()
    {
        gameTimeManager = FindObjectOfType<GameTimeManager>();
        shiza = FindObjectOfType<Shiza>();
        DialogUI.SetActive(false);   
        dialogManager = null;        
        isOver = true;
    }


    public void ToggleChoices()
    {
        _choicesVisible = !_choicesVisible;
        foreach (var btn in _buttons)
            btn.SetActive(_choicesVisible);

        if (arrowOpen  != null) arrowOpen.SetActive(!_choicesVisible);
        if (arrowClose != null) arrowClose.SetActive(_choicesVisible);
    }
    public void StartDialog()
    {
        //dialogManager = manager;
        isOver = false;

        DialogUI.SetActive(true);
        ShowCurrentDialog();
    }
    public void PreviousDialog()
    {
        dialogManager?.PreviousDialog();
        ShowCurrentDialog();
    }
    public void OPenIfNotOver()
    {
        if (!isOver && dialogManager != null)
        {
            DialogUI.SetActive(true);
            
        }
        else
        {
            DialogUI.SetActive(false);
        }
    }

    public void ShowCurrentDialog()
    {
        if (dialogManager?.currentDialog == null) return;
        
        choiceText.text = dialogManager.currentDialog.text;
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
        dialogManager.NextDialog(dialogManager.choices.IndexOf(choice));
        ShowCurrentDialog();
        
    }

    public void CloseDialog()
    {
        //NextDialogButton.interactable = true;
        dialogManager = null;
        choiceText.text = "";
        NPCManager.DeleteNPC();
        DialogUI.SetActive(false);
        foreach (var btn in _buttons) Destroy(btn);
        _buttons.Clear();
        isOver = true;
        gameTimeManager.AddTime(3, 0);
        shiza.OnDialogFinished();
    }
}