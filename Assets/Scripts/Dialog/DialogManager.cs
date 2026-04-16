using UnityEngine;
using System.Collections.Generic;

public class DialogManager : MonoBehaviour
{
    public string npcId;
    public DialogObject baseDialog; // объект диалога с которого начинаем

    public DialogObject currentDialog; // объект диалога который идет в данный момент 


    public List<Choice> choices; // список вариантов

    
    [SerializeField]
    private List<DialogObject>  _history = new(); // список уже выбранных диалогов
    

    void Awake()
    {
        // Инициализирую диалог здесь чтобы dialoguimanagerscript
        // мог безопасно обращаться к currentDialog и choices в своём Start()
        if (baseDialog == null) return;

        currentDialog = baseDialog;
        currentDialog.wasSelected = true;
        choices = currentDialog.choices;
        _history.Add(currentDialog);
    }
    void Start()
    {
        if (NPCDialogRegistry.Instance != null)
            NPCDialogRegistry.Instance.Register(npcId, this);
        else
            Debug.LogWarning($"[DialogManager] NPCDialogRegistry не найден на сцене.");
    }

    void OnDestroy()
    {
        if (NPCDialogRegistry.Instance != null)
        {
            NPCDialogRegistry.Instance.Unregister(npcId);
        }
    }
    
    // выбор следующего диалога по номеру из choices
    public void NextDialog(int choiceIndex)
    {
        if (choices == null || choiceIndex >= choices.Count) return;

        currentDialog = choices[choiceIndex].nextNode;
        _history.Add(currentDialog);
        choices = currentDialog.choices;
        currentDialog.wasSelected = true;
    }

    // переключение на предыдущую ветку диалога
    public void PreviousDialog()
    {
        if (_history.Count <= 1) return;

        currentDialog = _history[_history.Count - 2];
        _history.RemoveAt(_history.Count - 1);
        choices = currentDialog.choices;
    }
    
    // простая логика разблокировки диалога
    public void UnlockNode(DialogObject node)
    {
        if (node == null) return;
        node.isActive = true;
    }

    public void UnlockAllChoices()
    {
        if (choices == null) return;
        foreach (Choice choice in choices)
        {
            if (choice.nextNode != null && !choice.nextNode.isActive)
                choice.nextNode.isActive = true;
        }
    }
    
}
