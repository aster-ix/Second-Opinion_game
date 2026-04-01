using UnityEngine;
using System.Collections.Generic;

public class DialogManager : MonoBehaviour
{
    public DialogObject baseDialog; // объект диалога с которого начинаем
    public List<Choice> choices; // список вариантов

    public DialogObject currentDialog; // объект диалога который идет в данный момент 
    
    [SerializeField]
    private List<DialogObject>  _choices = new(); // список уже выбранных диалогов
    
    void Start()
    {
        // инициализируем начальный диалог
        
        if (baseDialog == null) return;
        
        baseDialog.wasSelected = true;
        currentDialog = baseDialog;
        choices = currentDialog.choices;
        _choices.Add(currentDialog);
        
    }

    
    // выбор следующего диалога по номеру из choices
    public void NextDialog(int choice)
    {
        if (choices != null)
        {
            _choices.Add(currentDialog);
            Debug.Log(choice);
            currentDialog = choices[choice].nextNode;
            choices = currentDialog.choices;
            currentDialog.wasSelected = true;
        }
    }

    // переключение на предыдущую ветку диалога
    public void PreviousDialog()
    {
        if (_choices.Count > 1)
        {
            currentDialog = _choices[_choices.Count - 1];
            _choices.RemoveAt(_choices.Count - 1);
            choices = currentDialog.choices;
        }
    }
    
    // простая логика разблокировки диалога
    public void UnlockDialog()
    {
        foreach (Choice choice in choices)
        {
            if (choice.nextNode.isActive == false)
            {
                choice.nextNode.isActive = true;
            }
        }
    }
    
}
