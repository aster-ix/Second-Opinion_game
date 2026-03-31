using UnityEngine;
using System.Collections.Generic;

public class DialogManager : MonoBehaviour
{
    public DialogObject baseDialog; // объект диалога с которого начинаем
    public List<Choice> choices; // список вариантов
    private DialogObject _currentDialog; // объект диалога который идет в данный момент 
    private List<DialogObject>  _choices = new(); // список уже выбранных диалогов
    
    void Start()
    {
        // инициализируем начальный диалог
        
        if (baseDialog == null) return;
        
        baseDialog.wasSelected = true;
        _currentDialog = baseDialog;
        choices = _currentDialog.choices;
        _choices.Add(_currentDialog);
        
    }

    
    // выбор следующего диалога по номеру из choices
    public void NextDialog(int choice)
    {
        if (choices != null)
        {
            _choices.Add(_currentDialog);
            _currentDialog = choices[choice].nextNode;
            choices = _currentDialog.choices;
            _currentDialog.wasSelected = true;
        }
    }

    // переключение на предыдущую ветку диалога
    public void PreviousDialog()
    {
        if (_choices.Count > 1)
        {
            _currentDialog = _choices[_choices.Count - 1];
            _choices.RemoveAt(_choices.Count - 1);
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
